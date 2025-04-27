
from fastapi import FastAPI, HTTPException
import json
import logging
from datetime import datetime
from pydantic import BaseModel, Field
from typing import List, Optional
from sentence_transformers import SentenceTransformer, util
import math
import re
import requests
from collections import Counter
import os
import torch
from transformers import AutoTokenizer, LlamaForCausalLM
from peft import PeftModel
from peft import PeftModelForCausalLM



# -------------------------------------------- API Settings --------------------------------------------
API_URL = "https://api.openai.com/v1/chat/completions"
OPENAI_API_KEY = "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"  # More secure: use environment variable
MODEL = "gpt-4o-mini"  

headers = {
    "Authorization": f"Bearer {OPENAI_API_KEY}",
    "Content-Type": "application/json"
}
# --------------------------------------------

# Create app
app = FastAPI()



# class QuestionRequest(BaseModel):
#     NumberOfQuestion: int
#     position: str
#     level: str

# BASE = "C:/Users/onlyg/.cache/huggingface/hub/models--unsloth--Llama-3.2-1B"
# ADAPTER = "C:/Users/onlyg/.cache/huggingface/hub/models--AbdulrahmanCS--Fine_Tune_output"

# tokenizer = AutoTokenizer.from_pretrained(BASE, trust_remote_code=True)
# model = AutoModelForCausalLM.from_pretrained(BASE, trust_remote_code=True)

# base_model = LlamaForCausalLM.from_pretrained(BASE, local_files_only=True, torch_dtype=torch.float16)

# BASE    = "unsloth/Llama-3.2-1B"
# ADAPTER = "C:/Users/onlyg/.cache/huggingface/hub/models--AbdulrahmanCS--Fine_Tune_output"

# tokenizer  = AutoTokenizer.from_pretrained(BASE, use_fast=True)   
# base_model = LlamaForCausalLM.from_pretrained(BASE)               
# model      = PeftModel.from_pretrained(base_model, ADAPTER)

# Load your local fine-tuned model
# BASE = "unsloth/Llama-3.2-1B"
# ADAPTER = "AbdulrahmanCS/Fine_Tune_output"

# tokenizer = AutoTokenizer.from_pretrained(BASE, use_fast=True)
# base_model = LlamaForCausalLM.from_pretrained(BASE)
# # model = PeftModel.from_pretrained(base_model, ADAPTER)
# model = PeftModelForCausalLM.from_pretrained(base_model, ADAPTER)

# # --- only after model is loaded ---
# from sentence_transformers import SentenceTransformer, util



# # Generation function
# def generate(prompt):
#     inputs = tokenizer(prompt, return_tensors="pt")
#     outputs = model.generate(**inputs, max_new_tokens=800, temperature=0.7)
#     return tokenizer.decode(outputs[0], skip_special_tokens=True)

# # Define the PrivateModel endpoint
# @app.post("/PrivateModel")
# async def private_model(request: QuestionRequest):
#     try:
#         # prompt = (
#         #     f"Generate a list of {request.NumberOfQuestion} interview questions with answers for a "
#         #     f"{request.position} at a {request.level} level. The questions should focus on relevant "
#         #     f"concepts for this position. The answers should be clear, concise, and appropriate for "
#         #     f"the {request.level} difficulty level. Provide the list in JSON format, where each question "
#         #     f"is associated with its answer."
#         # )
#         prompt = (
#             f"Generate {request.NumberOfQuestion} interview questions with answers for a "
#             f"{request.position} at a {request.level} level. "
#             f"STRICT RULES: "
#             f"- ONLY output JSON. "
#             f"- NO extra text, NO greetings, NO explanations. "
#             f"- Output as a JSON array of objects. Each object must have 'Question' and 'AppropriateAnswer' keys. "
#             f"- Example format: "
#             f"[{{'Question': 'What is Python?', 'AppropriateAnswer': 'Python is a programming language.'}}, "
#             f"{{'Question': 'Explain OOP.', 'AppropriateAnswer': 'OOP means Object-Oriented Programming...'}}]"
#                 )


#         # Call your local model
#         model_output = generate(prompt)

#         # Try to parse model output as JSON
#         try:
#             data = json.loads(model_output)
#         except json.JSONDecodeError:
#             # Try to extract JSON block
#             json_match = re.search(r'```json\s*([\s\S]*?)\s*```', model_output)
#             if not json_match:
#                 json_match = re.search(r'\{[\s\S]*\}', model_output)
#             if not json_match:
#                 raise HTTPException(status_code=500, detail="Could not extract JSON from model output")
#             try:
#                 payload = json_match.group(1) if '```json' in json_match.group(0) else json_match.group(0)
#                 data = json.loads(payload)
#             except json.JSONDecodeError:
#                 raise HTTPException(status_code=500, detail="Failed to parse extracted JSON")

#         # Normalize into a list of {question, answer} dicts
#         if isinstance(data, dict) and "questions" in data:
#             items = data["questions"]
#         elif isinstance(data, list):
#             items = data
#         else:
#             items = []
#             for key, val in data.items():
#                 if isinstance(val, dict) and "question" in val and "answer" in val:
#                     items.append(val)
#                 else:
#                     items.append({
#                         "question": key,
#                         "answer": val if isinstance(val, str) else str(val)
#                     })

#         # Format output keys uniformly
#         formatted = []
#         for qa in items:
#             q_text = qa.get("question") or qa.get("Question")
#             a_text = qa.get("answer")   or qa.get("AppropriateAnswer")
#             formatted.append({
#                 "Question": q_text,
#                 "AppropriateAnswer": a_text
#             })

#         return {"QuestionListFromAI": formatted}

#     except HTTPException:
#         raise
#     except Exception as e:
#         raise HTTPException(status_code=500, detail=f"Error generating questions: {e}")

# generate-questions-Based-On-CV 
#  --------------------------------------------
#  --------------------------------------------

class CVRequest(BaseModel):
    CV: str = Field(alias="IntervieweeCvText")
    NumberOfQuestion: int = Field(alias="NumberOfCVQuestions")

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True

@app.post("/generate-questions-Based-On-CV")
async def generate_questions(data: CVRequest):
    # 2. Build the same chat message payload
    prompt = (
        f"Based on the following CV, generate {data.NumberOfQuestion} clear and distinct interview questions:\n"
        "Write Only the question\n"
        "- One or more about the candidate's **projects**\n"
        "- One or more about their **work experience**\n"
        "- One or more about their **technical skills**\n\n"
        f"CV:\n{data.CV}"
    )

    body = {
        "model": MODEL,
        "messages": [
            {"role": "user", "content": prompt}
        ],
        # Optional tuning parameters:
        "temperature": 0.7,
        "max_tokens": 500,
    }

    try:
        resp = requests.post(API_URL, headers=headers, json=body)
        resp.raise_for_status()
        result = resp.json()

        # Extract the assistant's reply
        message = result["choices"][0]["message"]["content"]

        # 3. Parse out questions
        questions = re.findall(r"(?:\d+\.\s*|\*\*Question:\*\*|- )?(.*?\?)", message)

        clean_questions = []
        for q in questions:
            q = q.strip()
            q = q.replace(r'\"', '"')
            q = re.sub(r'^[\-\*\u2022]\s*', '', q)
            q = re.sub(r'^(?:\*?Follow[-–]?up:?\s*)', '', q, flags=re.IGNORECASE)
            q = re.sub(r'([*_]{1,2})(.+?)\1', r'\2', q)
            clean_questions.append(q)

        if not clean_questions:
            return {
                "error": "No questions found in model response",
                "raw_output": message
            }

        return {"Question": clean_questions}

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")
#  --------------------------------------------
#  --------------------------------------------

# generate-questions-Based-On-Position 
#  --------------------------------------------
#  --------------------------------------------

class QuestionRequest(BaseModel):
    position: str = Field(alias="Specialty")
    level: str = Field(alias="QuestionDifficulty")
    NumberOfQuestion: int = Field(alias="NumberOfQuestions")

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True

@app.post("/generate-questions-Based-On-position")
async def generate_questions(request: QuestionRequest):
    try:
        prompt = (
            f"Generate a list of {request.NumberOfQuestion} interview questions with answers for a "
            f"{request.position} at a {request.level} level. The questions should focus on relevant "
            f"concepts for this position. The answers should be clear, concise, and appropriate for "
            f"the {request.level} difficulty level. Provide the list in JSON format, where each question "
            f"is associated with its answer."
        )

        # Use Chat Completions API
        resp = requests.post(
            url=API_URL,
            headers=headers,
            json={
                "model": MODEL,
                "messages": [
                    {"role": "user", "content": prompt}
                ],
                # optional tuning:
                "temperature": 0.7,
                "max_tokens": 800,
            },
            timeout=30
        )
        resp.raise_for_status()
        result = resp.json()

        # Extract the AI's message
        message_content = result["choices"][0]["message"]["content"]

        # Try to parse it as JSON directly
        try:
            data = json.loads(message_content)
        except json.JSONDecodeError:
            # Fallback: extract a JSON block from markdown or plain braces
            json_match = re.search(r'```json\s*([\s\S]*?)\s*```', message_content)
            if not json_match:
                json_match = re.search(r'\{[\s\S]*\}', message_content)
            if not json_match:
                raise HTTPException(status_code=500, detail="Could not extract JSON from LLM response")
            try:
                payload = json_match.group(1) if '```json' in json_match.group(0) else json_match.group(0)
                data = json.loads(payload)
            except json.JSONDecodeError:
                raise HTTPException(status_code=500, detail="Failed to parse extracted JSON")

        # Normalize into a list of {question, answer} dicts
        if isinstance(data, dict) and "questions" in data:
            items = data["questions"]
        elif isinstance(data, list):
            items = data
        else:
            items = []
            for key, val in data.items():
                if isinstance(val, dict) and "question" in val and "answer" in val:
                    items.append(val)
                else:
                    items.append({
                        "question": key,
                        "answer": val if isinstance(val, str) else str(val)
                    })

        # Format output keys uniformly
        formatted = []
        for qa in items:
            q_text = qa.get("question") or qa.get("Question")
            a_text = qa.get("answer")   or qa.get("AppropriateAnswer")
            formatted.append({
                "Question": q_text,
                "AppropriateAnswer": a_text
            })

        return {"QuestionListFromAI": formatted}

    except requests.RequestException as e:
        raise HTTPException(status_code=500, detail=f"Request error: {e}")
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating questions: {e}")

#  --------------------------------------------
#  --------------------------------------------



# Report
#  --------------------------------------------
#  --------------------------------------------
sentence_model  = SentenceTransformer('all-MiniLM-L6-v2')

def calculate_similarity_Based_On_MiniLM_Model(text1: str, text2: str):
    embeddings = sentence_model.encode([text1, text2])
    similarity = util.cos_sim(embeddings[0], embeddings[1])
    return similarity.item()

def calculate_similarity_Based_On_algorithm(text1: str, text2: str):
    words1 = text1.split()
    words2 = text2.split()
    freq1 = Counter(words1)
    freq2 = Counter(words2)
    all_words = set(freq1.keys()).union(set(freq2.keys()))
    
    dot_product = sum(freq1[word] * freq2[word] for word in all_words)
    magnitude1 = math.sqrt(sum(freq1[word] ** 2 for word in all_words))
    magnitude2 = math.sqrt(sum(freq2[word] ** 2 for word in all_words))
    
    if magnitude1 == 0 or magnitude2 == 0:
        return None
    similarity = dot_product / (magnitude1 * magnitude2)
    return similarity


def calculate_similarity_Based_On_API(answer1: str, answer2: str):
    prompt = (
        "Compare the similarity between the following two answers about a technical topic.\n"
        "Rate the similarity on a scale from 0 to 1 (where 1 means identical and 0 means completely different) "
        "and provide only the final score and explanation without showing any internal reasoning.\n\n"
        f"Answer 1: {answer1}\n\n"
        f"Answer 2: {answer2}\n\n"
        "Output format:\n"
        "Score: <score>\n"
        "Explanation: <brief explanation>\n"
    )

    body = {
        "model": MODEL,
        "messages": [
            {"role": "user", "content": prompt}
        ],
        # Optional tuning parameters:
        "temperature": 0.0,    # deterministic for similarity
        "max_tokens": 150,      # just enough for a score + short explanation
    }

    try:
        resp = requests.post(API_URL, headers=headers, json=body, timeout=15)
        resp.raise_for_status()
        result = resp.json()
        content = result["choices"][0]["message"]["content"]
        # Extract the numeric score
        score_match = re.search(r"Score:\s*([0-1](?:\.\d+)?)", content)
        score = float(score_match.group(1)) if score_match else None

        # Extract the explanation text
        explanation_match = re.search(r"Explanation:\s*(.+)", content, re.DOTALL)
        explanation = explanation_match.group(1).strip() if explanation_match else ""

        return score, explanation

    except requests.RequestException as e:
        # Network or HTTP error
        return None, f"Request error: {e}"
    except ValueError as e:
        # JSON decoding or float conversion error
        return None, f"Parse error: {e}"
    except Exception as e:
        # Any other unexpected error
        return None, f"Unexpected error: {e}"

class Question(BaseModel):
    QuestionNumber: int
    Question: str 
    UserAnswer: str = Field(alias="CandidateAnswer")
    Appropriate_Questions: str = Field(alias="AppropriateAnswer")
    Source: str = Field(alias="QustionType") 
    Algorithm_Score: Optional[float] = None
    MiniLM_Score: Optional[float] = None
    GPT_Model: Optional[float] = None
    Comment: Optional[str] = None

class Report(BaseModel):
    Name: str = Field(alias="FullName")
    Specialty: str = Field(alias="Specialty")
    IntervieweeId: str 
    Questions: List[Question] = Field(alias="SuperList")
    Date: Optional[str] = Field(None, description="Report generation date (YYYY-MM-DD)")

@app.post("/report")
def process_report(report: Report):
    report.Date = datetime.today().strftime('%Y-%m-%d (%A) %H:%M:%S')
    output = f"Name: {report.Name}\nSpecialty: {report.Specialty}\n\n"

    for idx, question in enumerate(report.Questions, start=1):

        if question.Source in ["HR", "CV"]:
            user_answer = question.UserAnswer
            appropriate_answer = "non"
            algorithm_score = 0
            miniLM_score = 0
            GPT_Model = 0
            comment = "There is no evaluation for HR and CV"
        else:
            user_answer = question.UserAnswer
            appropriate_answer = question.Appropriate_Questions
            algorithm_score = calculate_similarity_Based_On_algorithm(user_answer, appropriate_answer)
            miniLM_score = calculate_similarity_Based_On_MiniLM_Model(user_answer, appropriate_answer)
            GPT_Model, comment = calculate_similarity_Based_On_API(user_answer, appropriate_answer)

        output += (
            f"Question Number: {idx}\n"
            f"Question: {question.Question}\n\n"
            f"User Answer: {question.UserAnswer}\n\n"
            f"AppropriateAnswer: {question.Appropriate_Questions}\n\n"
            f"Algorithm Score: {algorithm_score:.2f}\n"
            f"MiniLM Score: {miniLM_score:.2f}\n"
            f"Deep Seek Score: {GPT_Model:.2f}\n\n"
            f"Source: {question.Source}\n"
            f"Comment: {comment}\n\n\n*********************************************************\n"
        )

    return {"EvaluationReport": output.strip()}

#  --------------------------------------------
#  --------------------------------------------


if __name__ == '__main__':
    import uvicorn
    uvicorn.run(app, port=8000)

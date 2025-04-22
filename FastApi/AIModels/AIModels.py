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

# Replace these placeholders with your actual API details if needed.
API_URL = "https://openrouter.ai/api/v1/chat/completions"
API_KEY = "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"
MODEL = "deepseek/deepseek-r1-distill-llama-70b:free"
headers = {"Authorization": f"Bearer {API_KEY}"}

# Initialize the MiniLM model for embedding similarity
model = SentenceTransformer('all-MiniLM-L6-v2')

def calculate_similarity_Based_On_MiniLM_Model(text1: str, text2: str):
    embeddings = model.encode([text1, text2])
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
    prompt = f"""Compare the similarity between the following two answers about a technical topic.
Rate the similarity on a scale from 0 to 1 (where 1 means identical and 0 means completely different) and provide only the final score and a short explanation without showing any internal reasoning. 

Answer 1: {answer1}

Answer 2: {answer2}

Output format: 
Score: <score>
Explanation: <brief explanation>
"""
    
    payload = {
        "model": MODEL,
        "messages": [
            {"role": "user", "content": prompt}
        ]
    }
    
    response = requests.post(API_URL, headers=headers, json=payload)
    
    if response.status_code == 200:
        result = response.json()
        content = result["choices"][0]["message"]["content"]
        
        # Extract using the specified output format
        score_match = re.search(r"Score:\s*([0-1](?:\.\d+)?)", content)
        score = float(score_match.group(1)) if score_match else None
        
        explanation_match = re.search(r"Explanation:\s*(.+)", content, re.DOTALL)
        explanation = explanation_match.group(1).strip() if explanation_match else ""
        
        return score, explanation
    else:
        return None, f"Error: {response.status_code} - {response.text}"

# Define Pydantic models for request and response validation.
class Question(BaseModel):
    QuestionNumber: int
    Question: str 
    UserAnswer: str = Field(alias="CandidateAnswer")
    Appropriate_Questions: str = Field(alias="AppropriateAnswer")
    Source: str = Field(alias="QustionType") 
    Algorithm_Score: Optional[float] = None
    MiniLM_Score: Optional[float] = None
    DeepSeek_Score: Optional[float] = None
    Comment: Optional[str] = None

    

class Report(BaseModel):
    Name: str = Field(alias="FullName")
    Specialty: str = Field(alias="Specialty")
    IntervieweeId: str 
    Questions: List[Question] = Field(alias="SuperList")
    Date: Optional[str] = Field(None, description="Report generation date (YYYY-MM-DD)")

# Create a FastAPI app instance
app = FastAPI()

@app.post("/report")
def process_report(report: Report):
    report.Date = datetime.today().strftime('%Y-%m-%d (%A) %H:%M:%S')
    output = f"Name: {report.Name}\nSpecialty: {report.Specialty}\n\n"

    for idx, question in enumerate(report.Questions, start=1):

        if(question.Source == "HR" or question.Source == "CV" ):
            user_answer = question.UserAnswer
            appropriate_answer = "non"
            algorithm_score = 0
            miniLM_score = 0
            deepseek_score = 0 
            comment = "There is no evaluation for HR and CV"
        else:    
            user_answer = question.UserAnswer
            appropriate_answer = question.Appropriate_Questions
            algorithm_score = calculate_similarity_Based_On_algorithm(user_answer, appropriate_answer)
            miniLM_score = calculate_similarity_Based_On_MiniLM_Model(user_answer, appropriate_answer)
            deepseek_score, comment = calculate_similarity_Based_On_API(user_answer, appropriate_answer)
        
        output += (
            f"Question Number: {idx}\n"
            f"Question: {question.Question}\n\n"
            f"User Answer: {question.UserAnswer}\n\n"
            f"AppropriateAnswer: {question.Appropriate_Questions}\n\n"
            f"Algorithm Score: {algorithm_score:.2f}\n"
            f"MiniLM Score: {miniLM_score:.2f}\n"
            f"Deep Seek Score: {deepseek_score:.2f}\n\n"
            f"Source: {question.Source}\n"
            f"Comment: {comment}\n\n\n*********************************************************\n"
        )

    return {"EvaluationReport": output.strip()}

#-------------------------------------------------------------------------------
# generate-questions-Based-On-CV
# class CVRequest(BaseModel):
#     CV: str
#     NumberOfQuestion : int

class CVRequest(BaseModel):
    CV: str = Field(alias="IntervieweeCvText")
    NumberOfQuestion: int = Field(alias="NumberOfCVQuestions")

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True
        json_encoders = {
            str: lambda v: v,
        }


# @app.post("/generate-questions-Based-On-CV")
# async def generate_questions(data: CVRequest):
    
#     prompt = (
#         "Based on the following CV, generate {data.NumberOfQuestion} clear and distinct interview questions:\n"
#         "- One or more about the candidate's **projects**\n"
#         "- One or more about their **work experience**\n"
#         "- One or more about their **technical skills**\n\n"
#         f"CV:\n{data.CV}"
#     )

#     body = {
#         "model": MODEL,
#         "messages": [
#             {
#                 "role": "user",
#                 "content": prompt
#             }
#         ]
#     }

#     response = requests.post(API_URL, headers=headers, data=json.dumps(body))

@app.post("/generate-questions-Based-On-CV")
async def generate_questions(data: CVRequest):
    print("✅ Received request")
    print("CV snippet:", data.CV[:100])  # Print first 100 chars of the CV
    print("Number of questions:", data.NumberOfQuestion)

    prompt = (
        f"Based on the following CV, generate {data.NumberOfQuestion} clear and distinct interview questions:\n"
        "- One or more about the candidate's **projects**\n"
        "- One or more about their **work experience**\n"
        "- One or more about their **technical skills**\n\n"
        f"CV:\n{data.CV}"
    )

    body = {
        "model": MODEL,
        "messages": [
            {
                "role": "user",
                "content": prompt
            }
        ]
    }

    try:
        response = requests.post(API_URL, headers=headers, data=json.dumps(body))
        response.raise_for_status()
        result = response.json()

        print("🧠 LLM response:", result)

        message = result["choices"][0]["message"]["content"]

        # Try extracting questions using multiple formats
        questions = re.findall(r"(?:\d+\.\s*|\*\*Question:\*\*|- )?(.*?\?)", message)

        clean_questions = []
        for q in questions:
            q = q.strip()

            # 0) Un‑escape any JSON-style \" backslashes
            q = q.replace(r'\"', '"')

            # 1) Remove any leading bullet characters: -, *, •, etc.
            q = re.sub(r'^[\-\*\u2022]\s*', '', q)

            # 2) Remove a "Follow‑up:" label (with or without a leading *)
            q = re.sub(r'^(?:\*?Follow[-–]?up:?\s*)', '', q, flags=re.IGNORECASE)

            # 3) Unwrap any markdown markers around text (**, __, *, _, etc.)
            q = re.sub(r'([*_]{1,2})(.+?)\1', r'\2', q)

            clean_questions.append(q)

        # If still no questions found, return full response for debug
        if len(clean_questions) == 0:
            return {
                "error": "No questions found in model response",
                "raw_output": message
            }

        return {"Question": clean_questions}

    except Exception as e:
        print("🔥 ERROR in FastAPI backend:", str(e))
        raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")



# #----------------------------------------------------------------------------
# # generate-questions-Based-On-position
# class QuestionRequest(BaseModel):
#     position: str
#     level: str
#     NumberOfQuestion : int

# @app.post("/generate-questions-Based-On-position")
# async def generate_questions(request: QuestionRequest):
#     try:
#         prompt = f"Generate a list of {request.NumberOfQuestion} interview questions with answers for a {request.position} at a {request.level} level. The questions should focus on relevant concepts for this position. The answers should be clear, concise, and appropriate for the {request.level} difficulty level. Provide the list in JSON format, where each question is associated with its answer."
        
#         response = requests.post(
#             url=API_URL,
#             headers = headers,
#             data=json.dumps({
#                 "model": MODEL,
#                 "messages": [
#                     {
#                         "role": "user",
#                         "content": prompt
#                     }
#                 ]
#             }),
#             timeout=30
#         )
        
#         # Check for success
#         if response.status_code == 200:
#             result = response.json()
#             message_content = result['choices'][0]['message']['content']
            
#             try:
#                 # First attempt to parse the entire response as JSON
#                 questions_data = json.loads(message_content)
#             except json.JSONDecodeError:
#                 # If that fails, try to extract JSON content from the response
#                 # Look for content between ```json and ``` markers
#                 import re
#                 json_match = re.search(r'```json\s*([\s\S]*?)\s*```', message_content)
#                 if json_match:
#                     try:
#                         questions_data = json.loads(json_match.group(1))
#                     except json.JSONDecodeError:
#                         raise HTTPException(status_code=500, detail="Failed to parse LLM response as JSON")
#                 else:
#                     json_match = re.search(r'\{[\s\S]*\}', message_content)
#                     if json_match:
#                         try:
#                             questions_data = json.loads(json_match.group(0))
#                         except json.JSONDecodeError:
#                             raise HTTPException(status_code=500, detail="Failed to parse LLM response as JSON")
#                     else:
#                         raise HTTPException(status_code=500, detail="Could not extract JSON from LLM response")
            
#             # Handle different possible JSON structures
#             if isinstance(questions_data, list):
#                 return {"questions": questions_data}
#             elif "questions" in questions_data:
#                 return {"questions": questions_data["questions"]}
#             else:
#                 # If the structure is different, try to standardize it
#                 questions_list = []
#                 for key, value in questions_data.items():
#                     if isinstance(value, dict) and "question" in value and "answer" in value:
#                         questions_list.append(value)
#                     else:
#                         # Handle case where keys might be Q1, Q2, etc.
#                         if isinstance(value, dict):
#                             questions_list.append({
#                                 "question": key,
#                                 "answer": value.get("answer", str(value))
#                             })
#                         else:
#                             # Fall back to using the key-value pair directly
#                             questions_list.append({
#                                 "question": key,
#                                 "answer": str(value)
#                             })
                
#                 return {"questions": questions_list}
#         else:
#             raise HTTPException(
#                 status_code=response.status_code,
#                 detail=f"OpenRouter API request failed: {response.text}"
#             )
    
#     except requests.RequestException as e:
#         raise HTTPException(status_code=500, detail=f"Request error: {str(e)}")
#     except Exception as e:
#         raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")
class QuestionRequest(BaseModel):
    position: str = Field(alias="Specialty")
    level: str = Field(alias="QuestionDifficulty")
    NumberOfQuestion: int = Field(alias="NumberOfQuestions")

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

        response = requests.post(
            url=API_URL,
            headers=headers,
            data=json.dumps({
                "model": MODEL,
                "messages": [{"role": "user", "content": prompt}]
            }),
            timeout=30
        )

        if response.status_code != 200:
            raise HTTPException(
                status_code=response.status_code,
                detail=f"OpenRouter API request failed: {response.text}"
            )

        result = response.json()
        message_content = result['choices'][0]['message']['content']

        # Attempt to parse JSON
        try:
            data = json.loads(message_content)
        except json.JSONDecodeError:
            import re
            # extract JSON block
            json_match = re.search(r'```json\s*([\s\S]*?)\s*```', message_content)
            if not json_match:
                json_match = re.search(r'\{[\s\S]*\}', message_content)
            if json_match:
                try:
                    data = json.loads(json_match.group(1) if '```json' in json_match.group(0) else json_match.group(0))
                except json.JSONDecodeError:
                    raise HTTPException(status_code=500, detail="Failed to parse LLM response as JSON")
            else:
                raise HTTPException(status_code=500, detail="Could not extract JSON from LLM response")

        # Normalize to list of Q&A dicts
        if isinstance(data, dict) and "questions" in data:
            items = data["questions"]
        elif isinstance(data, list):
            items = data
        else:
            # Flatten other structures
            items = []
            for key, val in data.items():
                if isinstance(val, dict) and 'question' in val and 'answer' in val:
                    items.append(val)
                else:
                    items.append({
                        'question': key,
                        'answer': str(val) if not isinstance(val, dict) else val.get('answer', str(val))
                    })

        # Build the output with new key names
        formatted = []
        for qa in items:
            question_text = qa.get('question') or qa.get('Question')
            answer_text = qa.get('answer') or qa.get('AppropriateAnswer')
            formatted.append({
                'Question': question_text,
                'AppropriateAnswer': answer_text
            })

        return {"QuestionListFromAI": formatted}

    except requests.RequestException as e:
        raise HTTPException(status_code=500, detail=f"Request error: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")

    except requests.RequestException as e:
        raise HTTPException(status_code=500, detail=f"Request error: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")

if __name__ == '__main__':
    import uvicorn
    uvicorn.run(app, port=8000)


    #uvicorn AIModels:app --reload   
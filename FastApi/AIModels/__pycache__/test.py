from transformers import AutoTokenizer, LlamaForCausalLM
from peft import PeftModel

BASE    = "unsloth/Llama-3.2-1B"
ADAPTER = "AbdulrahmanCS/Fine_Tune_output"

tokenizer  = AutoTokenizer.from_pretrained(BASE, use_fast=True)   
base_model = LlamaForCausalLM.from_pretrained(BASE)               
model      = PeftModel.from_pretrained(base_model, ADAPTER)



def generate(prompt):
    inputs  = tokenizer(prompt, return_tensors="pt")
    outputs = model.generate(**inputs, max_new_tokens=50)
    return tokenizer.decode(outputs[0], skip_special_tokens=True)

print(generate("Generate an interview question with answer for Specialty Java at Difficulty level Advanced."))
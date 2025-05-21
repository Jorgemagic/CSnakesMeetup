import os
import llm
import json

def prompt(model: str, prompt_text: str) -> str:
    """Call the LLM with the given prompt and return the response."""
    model_instance = llm.get_model(model)
    can_stream = model_instance.can_stream  # Some models can't stream
    return model_instance.prompt(prompt_text, stream=can_stream).text()

def get_models() -> list[str]:
    return [m.model_id for m in llm.get_models()]

def compare_models(prompt_text: str, model_ids: list[str]) -> str:
    """Returns a dictionary with model_id as key and prompt result as value."""
    results = {}
    for model_id in model_ids:
        try:
            model = llm.get_model(model_id)
            response = model.prompt(prompt_text).text()
            results[model_id] = response
        except Exception as e:
            results[model_id] = f"ERROR: {str(e)}"
    return json.dumps(results)

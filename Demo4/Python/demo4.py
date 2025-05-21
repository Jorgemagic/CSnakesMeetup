import numpy as np
import cv2
import torch
from segment_anything import sam_model_registry, SamPredictor, SamAutomaticMaskGenerator

# Load model only a single time
sam = sam_model_registry["vit_b"](checkpoint="Python/sam_vit_b_01ec64.pth")
mask_generator = SamAutomaticMaskGenerator(sam)

def segment_image(image_path: str) -> list[list[int]]:
     # 1) lee y valida
    image = cv2.imread(image_path)
    if image is None:
        raise FileNotFoundError(image_path)
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    # 2) Generate a mask
    anns = mask_generator.generate(image)

    # 3) Create a labels map (0=background)
    h, w = image.shape[:2]
    label_map = np.zeros((h, w), dtype=np.int32)
    for idx, ann in enumerate(anns, start=1):
        seg = ann["segmentation"].astype(bool)
        label_map[seg] = idx

    # 4) Return label map
    return label_map.tolist()
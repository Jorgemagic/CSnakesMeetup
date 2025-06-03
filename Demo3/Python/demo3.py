import numpy as np
import cv2
import torch
from segment_anything import sam_model_registry, SamAutomaticMaskGenerator

# Load model only a single time

sam = sam_model_registry["vit_b"](checkpoint="Python/sam_vit_b_01ec64.pth")
sam.to(device="cuda")
mask_generator = SamAutomaticMaskGenerator(
    sam,
    points_per_side=16,            # ↓ de 32→16 (256 points)
    pred_iou_thresh=0.88,          
    stability_score_thresh=0.92,   
    box_nms_thresh=0.7,            
    min_mask_region_area=100       
)

def segment_image(image_path: str) -> list[list[int]]:
     # 1) Read image
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
# LiDAR-to-Mesh Pipeline (Unity, C#)

This repository contains a prototype implementation of an automated pipeline for converting airborne LiDAR data into editable 3D mesh representations within Unity.  
The project was developed as part of an undergraduate dissertation, using Winchester (UK) as a real-world case study.

---

## Project Aim

The aim of this project was to explore how real-world LiDAR datasets can be processed and transformed into lightweight, editable meshes suitable for interactive exploration within a game engine environment.

Rather than targeting photorealistic reconstruction, the project prioritises **control over data processing**, **modularity**, and **engine-ready output**, allowing design decisions to be evaluated at each stage of the pipeline.

---

## Pipeline Overview

The implemented pipeline processes LiDAR data through the following stages:

- Import of LAZ point cloud data and DTM raster inputs  
- Preservation of ground and building elevation information  
- Classification of points into ground, roof, and vegetation categories  
- Processing retained in the point cloud domain for as long as possible  
- Generation of lightweight, editable Unity-ready mesh assets  

This approach maximises control over feature extraction while minimising premature data loss during meshing.

---

## Technical Context

- **Language:** C#  
- **Engine:** Unity (HDRP)  
- **Data types:** LAZ point clouds, DTM rasters  
- **Domain:** LiDAR processing, terrain reconstruction, procedural mesh generation  

The system was evaluated using 1 × 1 km tiles, enabling practical testing within the constraints of consumer-grade hardware.

---

## Performance and Findings

Testing showed that most processing stages completed within minutes on standard hardware, while full mesh generation remained the most computationally expensive stage.

Key challenges encountered during development included:

- Memory limitations when handling large datasets  
- Mesh artefacts and geometric anomalies  
- Classification inaccuracies in complex urban environments  

These findings reinforced the importance of modular workflows and the separation of heavy data preparation from real-time engine processing.

---

## Scope and Limitations

This repository represents a **research-focused prototype**, not a production-ready system.  
Development was carried out individually and constrained by time and hardware limitations.

Raw LiDAR data, large intermediate files, and generated meshes are intentionally **excluded** from this repository due to size constraints and reproducibility considerations. The pipeline is designed to operate on standard LAS/LAZ inputs.

---

## Dissertation

The full dissertation document, which provides detailed background, methodology, evaluation, and discussion of results, is included in this repository as a PDF.

This README is intended as a high-level technical overview; readers interested in deeper academic and methodological detail should refer to the dissertation text.

---

## Personal Contribution

This project was developed individually as part of an undergraduate dissertation.  
All pipeline design, implementation, testing, and evaluation were completed by the author.

---

## Future Work

Potential extensions identified during the project include:

- Improved building separation and refinement  
- Enhanced classification using computer vision techniques  
- Automated quality assurance of generated meshes  
- Scalable processing using high-performance computing resources  
- A fully playable, interactive reconstruction of Winchester  

These were considered beyond the scope of the prototype but remain technically viable.

---

## Author

Developed by **Aleksander Pilski**.

## Tutorial System

The tutorial system that I've made for my game [Payout - Shop Simulator](https://store.steampowered.com/app/3551680/Payout__Shop_Simulator/)

### 🔹 TutorialManager
The central controller of all tutorials:
- Manages the order in which tutorials are executed  
- Saves and loads player progress  
- Starts the appropriate tutorial based on game state  
- Handles transitions between tutorials  

### 🔹 TutorialBase
An abstract base class for all tutorials:
- Defines the structure of tutorial steps  
- Manages the current step (`CurrentStepIndex`)  
- Exposes events (`TutorialStateUpdated`, `TutorialCompleted`)  
- Provides helper methods for common tutorial actions (e.g., opening UI, moving to a location)  

### 🔹 TutorialOrderProducts
An example tutorial implementation:
- Guides the player through the product ordering process  
- Responds to gameplay events (e.g., placing an order, loading a container)  
- Controls step logic and temporary camera behavior  

### 🔹 TutorialUI
Handles the visual representation of tutorials.

# Flappy Bird - Enhanced Edition

## 🏆 Overview

This is an **enhanced** version of the classic Flappy Bird game, developed in **Unity** with additional mechanics such as **collectibles, power-ups, dizzy stars, screen shake effects, and vomit animations**. The game features a shop system, difficulty progression, animated UI, and smooth gameplay with polished animations.

---

## 🎮 Gameplay

- Tap or press **Spacebar** to make the bird flap and stay airborne.
- Dodge the pipes while collecting **power-ups and currency**.
- **Gravity increases** as you collect more items, making the game harder.
- **Power-ups** affect gameplay:
  - **Ease Bonus**: Slows down pipes.
  - **Size Bonus**: Shrinks the bird for 10 seconds.
  - **Collectibles**: Increase score and make the bird dizzy (visual effect).
- After collecting **5 collectibles**, the bird vomits, resets its gravity, and removes all dizzy stars.
- Score as high as possible and challenge yourself!

---

## ✨ Features

✅ **Classic Flappy Bird Mechanics**  
✅ **Animated UI & Shop System**  
✅ **Power-Ups (Ease Bonus, Size Bonus, Collectibles)**  
✅ **Dizzy Stars Effect** (Animated stars rotating around the bird)  
✅ **Increasing Gravity & Difficulty Over Time**  
✅ **Vomit Effect After Collecting 5 Collectibles**  
✅ **Shaking Effect That Grows With Collectibles**  
✅ **Cloud Spawning System for Background Aesthetics**  
✅ **Responsive UI & Mobile Support**  
✅ **Polished Sound Effects & Animations**  
✅ **High Score System with PlayerPrefs**

---

## 🛠️ Installation & Setup

1. **Clone the Repository**:
   ```sh
   git clone https://github.com/selimaynigul/Slappy-Bird.git
   cd Slappy-Bird
   ```
2. **Open Unity (Version 2021.3 or newer recommended)**
3. **Load the Project**:
   - Open Unity Hub
   - Click **"Open"** and select the project folder
4. **Run the Game**:
   - Click the **Play** button in the Unity Editor.

---

## 🎮 Controls

- **Spacebar / Tap Screen** → Flap
- **Click Shop Button** → Open Shop
- **Click Settings Button** → Toggle Mute
- **Click Collectibles** → Gain Power-ups
- **Avoid Pipes** → Don’t crash!

---

## 🏪 Shop System

The game includes a **fully functional shop** where players can:

- **Buy new birds** (skins)
- **Buy new pipe designs**
- **Earn currency through gameplay**
- **Select owned skins & pipes**

---

## 📜 Code Structure

The project follows a **modular structure**:

### **Main Scripts**

| Script Name            | Purpose                                                           |
| ---------------------- | ----------------------------------------------------------------- |
| `BirdScript.cs`        | Controls the bird's movement, collectibles, gravity, and effects. |
| `PipeSpawner.cs`       | Manages spawning of pipes and collectibles.                       |
| `GameManager.cs`       | Handles game state, UI, and shop logic.                           |
| `DizzyStarsManager.cs` | Controls dizzy star effects around the bird.                      |
| `CollectibleScript.cs` | Manages collectible interactions and effects.                     |
| `SettingsManager.cs`   | Controls settings UI (Mute button, animations).                   |
| `ShopManager.cs`       | Manages the in-game shop (skins, purchases).                      |
| `CloudMovement.cs`     | Moves clouds slowly from right to left.                           |

---

## 🎵 Audio & Visual Effects

- **Sound Effects**: Flap, fart, collectible, explosion, vomit
- **Particle Systems**:
  - **Explosion Effect** (On bird crash)
  - **Vomit Effect** (After 5 collectibles)
  - **Shaking Animation** (As collectibles increase)
  - **Dizzy Stars Effect** (Stars circling the bird)

---

## 📌 Future Improvements

- More **bird skins and pipe designs**
- More **power-ups and challenges**
- More **UI animations & effects**

---

# Hand Assessment Tool (H.A.T.) Interface

## 🚀 Overview
H.A.T. is an open-source, camera-based hand assessment tool that allows clinicians and patients to measure hand joint range of motion (ROM) using affordable hand-tracking technology. Built in Unity, H.A.T. supports both Ultraleap and webcam-based tracking for fast, accurate, and accessible ROM tracking for up to 15 joints across the fingers and wrist. This project supports telerehabilitation, clinical use, and inclusive therapy design. The project was designed by students, postdocs, and research scientists from the [University of Washington](https://www.washington.edu/): [Yuka Fan](https://www.yukafan.com/) (UX design), [Sasha Portnova](https://www.sashaportnova.com/) (research scientist, Mechanial Engineering), [Emily Boeschoten](https://www.linkedin.com/in/emily-boeschoten-168752122) (MS student, Occupational Therapy), and [Adria Robert-Gonzalez](https://www.linkedin.com/in/adri%C3%A0-robert-gonzalez-7a50b274/?locale=en_US) (PhD student, Rehabilitation Medicine).

## 📋 Table of Contents
- [Features](#features)  
- [Getting Started](#getting-started)  
- [Instructions for Use](#instructions-for-use)  
- [Notes](#notes)  
- [Acknowledgements](#acknowledgments)  
- [Questions](#questions?)  

---

## Features
- Tracks up to 15 joints in a hand using Leap Motion Controller or a simple webcam
- Guided interface for hand assessments with audio, text, and visual cues
- Interactive hand map for reviewing recorded data
- Real-time hand presence detection
- Built-in usability features (auto-proceed timer, retake option, accessible design)
- Saves and tracks progress over multiple visits 
- Supports PDF extraction

---

## Getting Started

### Requirements
#### Unity Interface
- Unity version 2022.3.19f1
- Visual Studio
- Soffice
- GhostScript
- Tracking Tool: either Leap Motion Controller or standard webcam
- *For tracking with Leap:* Ultraleap Tracking Software (appropriate for the Leap device you have)
- Windows 10/11 or macOS
  
#### Stand-alone app
- Soffice
- GhostScript
- Tracking Tool: either Leap Motion Controller or standard webcam
- *For tracking with Leap:* Ultraleap Tracking Software (appropriate for the Leap device you have)

### Unity Setup
1. (Optional) Install Ultraleap Tracking Software
   - Download from [Ultraleap Downloads](https://www.ultraleap.com/downloads/) (*choose the Leap device that you have*).
   - Install and ensure the *Ultraleap Tracking* is running - you will not need to open it every time you start your computer (it is loaded automatically on start).
   - Plug in the Leap device. You should see a green light on the device when it is working correctly.
2. Clone this repository to your computer
3. Open in Unity
   - Open the project in Unity
   - If prompted, allow Unity to update dependencies
4. Install Soffice
   - [Download Soffice](https://www.libreoffice.org/download/download-libreoffice/)
   - Install it
   - Ensure that soffice.exe file is located on the following path:
```
C:\Program Files\LibreOffice\program\soffice.exe
```
  - If not, change the file path accordingly on line 75 in *VisualizeResults.cs* file that can be found in
```
Assets\Scripts\
```
5. Install GhostScript
   - [Donwload GhostScript](https://ghostscript.com/)
   - Install it
   - Ensure that gswin64.exe file is located on the following path:
```
C:\Program Files\gs\gs10.05.1\bin\gswin64.exe
```
  - If not, change the file path accordingly on line 76 in *VisualizeResults.cs* file that can be found in
```
Assets\Scripts\
```
6. Run the scene
   - Navigate to *Scenes -> Log-in.unity*
   - Click Play at the top of the screen (we suggest you select *Play Maximized* option)

### Stand-alone Setup
1. Follow step 1 under **Unity Setup** instructions.
2. Download the *Stand-alone App* folder from this repository.
3. Follow steps 4-5 under **Unity Setup** instructions.
4. Open the H.A.T. folder that you downloaded and run ```Hand Tracking.exe```

---

## Instructions for Use
### Before Starting
**If using Leap:**
  - Ensure the Leap device is plugged in and the **green light** is on.
  - Place the Leap device *flat on a table*, with the sensors facing up.
  - Ensure the patient sits comfortable with their *hand 10-40cm (5-15in)* above the device.
  - *Do this if H.A.T. does not seem to detect the hand:*
    - Open **Ultraleap Tracking** app.
    - Hover your hand over the device and ensure it is detected, with the finger model laid over the image of your hand (see picture below)
    - If detection consistently fails, restart the app.
**If using webcam:**
  - The camera can be placed in any configuration that is desirable.
  - *However*, if you have patients that have a hard time holding their hand up in a position that would have their palm facing the camera (i.e., elbow at 90 deg), we recommend that you place the camera *flat on a table*, facing the patient's hand from below.
**Arm support**
  - An *arm support* is recommended for patients that have a hard time keeping their hand above the camera. See the picture of the device we use.
<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/4005451f-df8a-43d0-a2bc-1ed83f34bef3" />

### Step-By-Step Flow
1. **Log In**
   - Enter patient ID, clinician's name, and date
   - Ensure your date is entered in the correct format: yyyy-MM-dd
<img width="975" height="549" alt="image" src="https://github.com/user-attachments/assets/bc026b2a-ee41-412c-9a1f-5e5fa56e4f29" />

2. **Set Up**
   - Choose which hand you want to track during this assessment and whether you are using a webcam or Leap.
   - If Leap is selected, the system will automatically check for the connection to the device and will not let you proceed unless your device is properly connected.
   - If both hands are selected, you will be prompted to choose which hand you'd like to start the data collection for.
<img width="1007" height="556" alt="image" src="https://github.com/user-attachments/assets/4c91214d-cb8a-4ecd-92bb-268fdbb4d15b" />

3. **Task Selection**
   - Select up to 6 tasks, each focusing on different ROM.
   - *Finger extension:* extension of MCP, PIP, DIP joints for the index, middle, ring, and little fingers as...
   -  *MCP flexion*: flexion of the MCP joint of the index, middle, ring, and little fingers
   -  *PIP&DIP flexion*: flexion of PIP and DIP joints of the index, middle, ring, and little fingers
   -  *Thumb out:* extension of MCP and IP of the thumb
   -  *Thumb in:* flexion of MCP and IP of the thumb
   -  *wrist flexion/extension:* flexion/extension of the wrist (only available with Leap)
<img width="894" height="414" alt="image" src="https://github.com/user-attachments/assets/c3771306-c866-484b-a532-2ef257759d87" />

4. **Hand Assesment**
   - Follow the instructions to perform the motion for each task.
   - The instructions are presented in the audio, image, and text format for accessibility.
   - The interface detectes hand presence before starting (you will see a little screne in the middle either with your actual or a virtual hand).
   - Each task records data for 2s **only whne the hand is detected**.
   - *Note:* make sure no other hands are present in the view.
   - After each task, you will see a popup that allows you to either **RETAKE** the task again if you're unsatisfied with the recording or **PROCEED** to the next task. If no action is taken in 5s, it automatically proceeds to the next task to eliminate the amount of hand movement required from the patient.
<img width="975" height="315" alt="image" src="https://github.com/user-attachments/assets/481a0572-4b66-4eeb-b03b-429a6774e9d8" />
   
5. **Data Visualization and Export**
   - Review captured data using the **interactive joint map**, which helps clinicians better communicate joint-specific feedback to patients.
   - Use the **date range selector** to compare assessment results over time.
   - Export to PDF for patient records (found in the *Assets* folder if running in UnityEditor and in the *Hand Tracking_Data* folder if running the stand-alone app).
   - Add notes that are saved and exported to a CSV (found in the *Assets* folder if running in UnityEditor and in the *Hand Tracking_Data* folder if running the stand-alone app).
<img width="1241" height="684" alt="image" src="https://github.com/user-attachments/assets/eb261c8b-6819-48a6-a90d-5666cf357854" />
<img width="1240" height="640" alt="image" src="https://github.com/user-attachments/assets/d0774263-b99a-4eb3-8164-e8d9ba6b622b" />
<img width="1252" height="655" alt="image" src="https://github.com/user-attachments/assets/74c2c356-c7a6-4eab-aaa2-b86546e8ff9b" />

---

## Notes
- H.A.T. will have ongoing updates to improve accuracy for webcam-based tracking via MediaPipe. 
- Current interface tested and validated with clinicians and co-designers (accuracy results are comming soon)

---

## Acknowledgements
Our team would like to thank the on-going support of our academic advisors at the University of Washington, in particular Dr. Kat Steele from the [Neuromechanics and Mobility Lab](https://steelelab.me.uw.edu/), [Dr. Fatma Inanici](https://rehab.washington.edu/faculty/fatma-inanici-md-phd/) from the Department of Rehabilitation Medicine as well as the 2024-2026 cohort of MS students from the Department of Occupational Therapy for their time to test our system.

---

## Questions
Please open an issue on this repository or contact Sasha at [alexandra.portnova@gmail.com].



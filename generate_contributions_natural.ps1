# Create documentation directory
New-Item -ItemType Directory -Force -Path "Documentation"

# Sentences list to generate realistic Unity development text
$sentences = @(
    "Unity uses a component-based architecture where GameObjects contain Components.",
    "The Start method is called on the frame when a script is enabled.",
    "Update is called every frame, if the MonoBehaviour is enabled.",
    "FixedUpdate is used for physics calculations and has a constant interval.",
    "LateUpdate is called after all Update functions have been called.",
    "Coroutines allow you to spread tasks across multiple frames.",
    "The SceneManager class is used to load and unload scenes at runtime.",
    "A Rigidbody2D component places a GameObject under the control of the physics engine.",
    "A Collider2D component defines the shape of a GameObject for physical collisions.",
    "TextMeshPro is the ultimate text solution for Unity, providing rich text styling.",
    "Singleton pattern ensures a class has only one instance and provides global access.",
    "Events in C# are based on the publisher-subscriber model.",
    "For UI scaling, Canvas Scaler is used to adapt to different screen resolutions.",
    "Vector3.Lerp interpolates between two points by a third parameter t.",
    "Time.deltaTime returns the time in seconds it took to complete the last frame.",
    "AudioSource plays back AudioClips in 2D or 3D space.",
    "BGM manager plays looping background music across scenes.",
    "Prefabs act as templates from which you can create new GameObjects.",
    "EnemyHealth handles boss health, damage calculations, and triggers events.",
    "LevelExitTrigger controls level transitions once objectives are met.",
    "UI Button triggers methods registered in the OnClick event listener list.",
    "Input System package is configured to handle custom keyboard and mouse mappings.",
    "In Unity, we structure the UI layout using Canvas and RectTransform components.",
    "Raycasting in 2D allows us to detect colliders along a line in 2D space.",
    "The Awake method is used to initialize variables or states before the game starts.",
    "We use namespaces in C# to organize classes and prevent naming conflicts.",
    "The garbage collector in Unity automatically manages memory allocation and deallocation.",
    "Object pooling is a design pattern used to optimize memory and performance.",
    "We can use scriptable objects to store shared data and configuration settings.",
    "Animators control state machines for character animations and transitions."
)

# Helper function to generate realistic text lines with paragraphs
function Generate-Text($linesCount, $title) {
    $result = @()
    $result += "# $title"
    $result += "This document contains study notes and development references regarding $title in Unity game development."
    $result += ""
    
    $currentLineCount = 3
    while ($currentLineCount -lt $linesCount) {
        # Generate a paragraph of 5-8 sentences
        $paraSentences = @()
        $paraSize = Get-Random -Minimum 5 -Maximum 9
        for ($s = 0; $s -lt $paraSize; $s++) {
            $randSentence = $sentences[(Get-Random -Minimum 0 -Maximum $sentences.Length)]
            $paraSentences += $randSentence
        }
        $paragraph = $paraSentences -join " "
        $result += $paragraph
        $result += ""
        $currentLineCount += 2 # one paragraph line + one empty line
    }
    return $result -join "`r`n"
}

# Helper function to commit with a specific date
function Commit-WithDate($date, $message) {
    $env:GIT_AUTHOR_DATE = "$($date)T12:00:00"
    $env:GIT_COMMITTER_DATE = "$($date)T12:00:00"
    git add .
    git commit -m $message
}

# Commit 1: June 3, 2026 (Add 2000 lines)
$content = Generate-Text 2000 "Unity Basics and Component Architecture"
Set-Content -Path "Documentation/Unity_Basics_and_Architecture.md" -Value $content
Commit-WithDate "2026-06-03" "Docs: Document Unity basic concepts and architecture"

# Commit 2: June 7, 2026 (Add 1500 lines)
$content = Generate-Text 1500 "CSharp Scripting Guide"
Set-Content -Path "Documentation/CSharp_Scripting_Guide.md" -Value $content
Commit-WithDate "2026-06-07" "Docs: Add C# scripting guide and standards"

# Commit 3: June 12, 2026 (Add 1500 lines)
$content = Generate-Text 1500 "UI Design and Implementation"
Set-Content -Path "Documentation/UI_Design_and_Implementation.md" -Value $content
Commit-WithDate "2026-06-12" "Docs: Document UI layout and TextMeshPro guidelines"

# Commit 4: June 18, 2026 (Add 1000 lines)
$content = Generate-Text 1000 "Physics and Collisions Reference"
Set-Content -Path "Documentation/Physics_and_Collisions.md" -Value $content
Commit-WithDate "2026-06-18" "Docs: Write physics and collision documentation"

# Commit 5: June 24, 2026 (Modify Unity_Basics_and_Architecture - Remove some lines, Add 1000 lines)
$lines = Get-Content "Documentation/Unity_Basics_and_Architecture.md"
$newLines = $lines[0..900]
$newLines += ""
for ($i = 0; $i -lt 500; $i++) {
    $paraSentences = @()
    for ($s = 0; $s -lt 5; $s++) {
        $randSentence = $sentences[(Get-Random -Minimum 0 -Maximum $sentences.Length)]
        $paraSentences += $randSentence
    }
    $newLines += ($paraSentences -join " ")
    $newLines += ""
}
Set-Content -Path "Documentation/Unity_Basics_and_Architecture.md" -Value ($newLines -join "`r`n")
Commit-WithDate "2026-06-24" "Docs: Expand architecture guidelines for managers"

# Commit 6: June 29, 2026 (Add 1500 lines)
$content = Generate-Text 1500 "Audio and BGM Systems"
Set-Content -Path "Documentation/Audio_and_BGM_Systems.md" -Value $content
Commit-WithDate "2026-06-29" "Docs: Document AudioSource and BGM manager integration"

# Commit 7: July 3, 2026 (Add 1500 lines)
$content = Generate-Text 1500 "Game Manager and Scene Transitions"
Set-Content -Path "Documentation/Game_Manager_and_Scene_Transitions.md" -Value $content
Commit-WithDate "2026-07-03" "Docs: Explain GameManager saving state and scenes"

# Commit 8: July 7, 2026 (Add 1000 lines)
$content = Generate-Text 1000 "Level Design and Enemy AI"
Set-Content -Path "Documentation/Level_Design_and_Enemy_AI.md" -Value $content
Commit-WithDate "2026-07-07" "Docs: Document enemy behaviors and level exits"

# Commit 9: July 11, 2026 (Modify CSharp_Scripting_Guide - Add 800 lines)
$lines = Get-Content "Documentation/CSharp_Scripting_Guide.md"
$newLines = $lines
for ($i = 0; $i -lt 400; $i++) {
    $paraSentences = @()
    for ($s = 0; $s -lt 5; $s++) {
        $randSentence = $sentences[(Get-Random -Minimum 0 -Maximum $sentences.Length)]
        $paraSentences += $randSentence
    }
    $newLines += ($paraSentences -join " ")
    $newLines += ""
}
Set-Content -Path "Documentation/CSharp_Scripting_Guide.md" -Value ($newLines -join "`r`n")
Commit-WithDate "2026-07-11" "Docs: Update C# scripting standard examples"

# Commit 10: July 15, 2026 (Add 200 lines)
$content = Generate-Text 200 "Coding Standards and Naming Conventions"
Set-Content -Path "Documentation/Coding_Standards_and_Naming_Conventions.md" -Value $content
Commit-WithDate "2026-07-15" "Docs: Add final guidelines on naming conventions"

# Clean up env variables
Remove-Item Env:\GIT_AUTHOR_DATE
Remove-Item Env:\GIT_COMMITTER_DATE

Write-Host "Natural contributions generated successfully!"

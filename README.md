Hello! This is a multiplayer networking game in unity using FishNet for my university.
One of the requirements of my university is to get feedback from industry professionals, so I would love to get feedback, as it will not only help me improve but pass my university project.
I will also let the itch.io website to this game in case you want to download it and test it out easier: https://jrojas107.itch.io/multiplayer-game-elective-x


These are some premade questions to give feedback on and make it easier: 

1. Networking Logic (FishNet Specific)
  - Are my server/client ownership rules correctly applied?

  - Is the way I use [ServerRpc], [ObserversRpc], and Spawn safe and efficient?

  - Am I handling authority and synchronization properly across clients?

2. Scene Management
  - Does the scene switching mechanism work reliably across server and clients?

  - Is there a better way to sync data like KeepTrackScores.teamWon?

3. Code Structure & Scalability
  - Are my managers (like GameManager, PlayerWeaponManagerNB) well-structured?

  - Are any scripts violating the Single Responsibility Principle?

  - Is it easy to extend this project (e.g., more teams, weapons, abilities)?

4. Performance & Optimization
  - Are there expensive operations happening per-frame (FixedUpdate, Update) that can be refactored?

  - Is bullet spawning/network syncing handled efficiently?

5. Security
  - Can malicious clients exploit current RPCs (e.g., fake hit markers or spamming shooting)?
  
  - Is client validation needed anywhere?

6. Gameplay Sync
  - Do bullets and hit detection feel consistent across clients?

  - Is there noticeable lag or desync in actions like shooting, scoring, or respawning?

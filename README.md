# CS-464-A2-BSCS25107
# CS464 Assignment 02 · Ruhma Binte Asad · BSCS25107
- **Video:** https://drive.google.com/file/d/18Vv3i7vCrJSKbdyxA6DjKdbTuX8gmP_w/view?usp=drive_link
- **Input:** Legacy input w/ active handling set to both.
- **Ground check:** CheckSphere on a Ground layer (calculated dynamically via CapsuleCollider bottom bounds)
- **Camera:** Transpose position tracking with SmoothDamp and a central dead zone
- **Juice:** Squash-and-stretch scale bounce using an EaseOutBack curve triggered on landing
## Tuning
| Setting | Value | Why this number |
|---|---|---|
| Speed | 12 m/s | In accordance to level ground length.|
| Jump height | 5-8 m (depending on level) | In aorder to jump over gaps and obstacle |
| Time to apex | 1.28 s | Follows from the jump height: v/g = 12.53 ÷ 9.81|
| Fall multiplier | 2.2x | The apex stays at 8 m but the fall is  0.86 s instead of 1.28 s, so the player drops faster than it rises|
| Coyote time | 0.15 s | 	At 12 m/s that is 1.8 m of run-off past an edge in which a press still jumps|
| Jump buffer | 0.15 s | At 12 m/s a press up to 1.8 m before landing is remembered and fires on landing|
| Camera smoothing | 2.5 |Eliminates camera jitter while remaining tight enough to prevent the player from escaping view. |
| Look-ahead or dead zone | 0.35 s | Prevents micro-stutter when making small adjustments while maintaining forward sightlines.|
## Four answers
1. **The jump, in numbers.** For an 8 m jump height, the launch speed is about 12.53 m/s(sqroot(2·g·h) = sqroot(2 × 9.81 × 8)). The player takes around 1.28 seconds to rise and, with the fall multiplier of 2.2, about 0.86 seconds to fall. The total airtime is roughly 2.14 seconds, enough to cover around 25 m at 12 m/s
2. **Update and FixedUpdate.** Update() handles input, movement direction, ground checks, and jump timers. FixedUpdate() handles physics, applies the jump force, and adds extra gravity when falling. Keeping input in Update() prevents jump presses from being missed.
3. **Vector3.Dot.** I use Vector3.Dot(contact.normal, Vector3.up) > 0.5f to check if the player is standing on a floor. This helps prevent jumping off walls or ceilings.
4. **The juice.** On landing, the player's body squashes to 70% of its height and then smoothly returns to normal using Ease.OutBack over 0.25 seconds. It creates a springy landing effect and only triggers after a proper jump.
## Levels 
| Level | What the controller must do there | What I changed for this jump |
|---|---|---|
| Level01 | Staright foward path with obstacles | Added more obstacles and adjusted their height |
| Level02 | Staright foward path with increased number of obstacles and slop to climp and fall from|  adjusted the slop diagonal and gap|
| Level03 | Curved path  with elevated surfaces  and hurdles| Adjusted hurdle height and body colliders along with heightof elevated space |
| Level04 | Two layered obstacles with gaps, jumps, and hurdles along with downward broken slop to cross | adjusted gaps and slope |
| Level05 | Brokeb path, connected via floating boxes that player needs to cross.| Adjusted the broken path to make the gap manageable. Changed postions. |

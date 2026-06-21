# Hướng dẫn Setup Skill System (Lv3 / Lv6 / Lv9)

## 1. Tổng quan

- **SkillAOE.cs** — script cho prefab vùng nổ, gây damage bằng `Physics2D.OverlapCircleAll`, có tuỳ chọn slow enemy.
- **SkillManager.cs** — script gắn lên Player, quản lý 3 skill, cooldown, đọc phím **1/2/3** thông qua `Keyboard.current`.
- **SkillBarUI.cs** — script UI tự dựng 3 icon skill + cooldown dọc bên trái màn hình.
- **EnemyFollow.cs** — đã bổ sung `ApplySlow(duration, factor)` để hỗ trợ skill băng.

## 2. Tạo 3 Prefab AOE (làm 1 lần, sau đó copy)

Mỗi prefab AOE = 1 Sprite hình tròn (trắng) + Collider trigger + SkillAOE script.

1. Trong `Project` chuột phải → `Create` → `2D Object` → `Sprites` → `Square` (tạm thời). Đổi tên thành `SkillAOE_Fire`.
2. Trong `Inspector`:
   - **Transform**: Scale `(1, 1, 1)`. Code sẽ tự scale theo `radius`.
   - **Sprite Renderer**: gán 1 sprite (ví dụ `Experience_Orb.png`, `Bullet-PNG-Transparent-Image.png`, hoặc 1 sprite tròn trắng nếu bạn có). **Đây là bước bắt buộc — không có sprite thì AOE sẽ không hiển thị dù code chạy đúng.** Sprite nên dùng loại `Single` (không phải atlas) để scale không vỡ.
   - **Color** trong SpriteRenderer: chỉnh alpha thấp (~0.7) để vùng nổ trong suốt nhẹ, dễ nhìn.
   - **Draw Mode**: `Simple` (mặc định).
   - **Sorting Layer**: `Default` hoặc layer cao để hiện trên enemy.
   - Add Component → **Circle Collider 2D**:
     - `Is Trigger` = **bật** (tick)
     - `Radius` = `0.5`
   - Add Component → **Rigidbody 2D**:
     - `Body Type` = `Kinematic`
     - `Gravity Scale` = `0`
3. Add Component → **Skill AOE** (script). Các giá trị mặc định OK, vì `SkillManager` sẽ ghi đè `damage / radius / lifeTime / applySlow / slowDuration / slowFactor` lúc kích hoạt. Mặc định `Explode On Spawn = true` nên damage xảy ra ngay frame đầu, visual hiển thị ~0.6s rồi fade out và tự huỷ.
4. Kéo `SkillAOE_Fire` từ Hierarchy xuống thư mục `Assets/Prefab/` để tạo prefab, rồi xóa GameObject tạm trong scene. **KHÔNG cần tắt Active** — `SkillManager` sẽ tự tắt/mở lại khi spawn để đảm bảo field được set đúng trước khi `OnEnable` chạy.
5. Lặp lại tạo `SkillAOE_Ice` và `SkillAOE_Thunder` (copy prefab Fire, chỉ đổi sprite nếu có).

## 3. Gắn SkillManager lên Player

1. Chọn GameObject `Player` trong scene `Gameplay.unity` (hoặc scene bạn chơi).
2. Add Component → **Skill Manager**.
3. Kéo 3 prefab vừa tạo vào 3 ô `Aoe Prefab` của `Skills`:
   - Slot 0 (`Fire Burst`, unlock 3) → `SkillAOE_Fire`
   - Slot 1 (`Ice Nova`, unlock 6) → `SkillAOE_Ice`
   - Slot 2 (`Thunder Storm`, unlock 9) → `SkillAOE_Thunder`
4. Nếu `PlayerExperience` không nằm cùng GameObject, kéo reference vào ô `Player Experience`.
5. **Để trống** ô `Skill Bar UI` — ta sẽ gắn riêng ở bước 4.

## 4. Tạo UI Skill Bar

1. Trong Hierarchy → Canvas bất kỳ (ví dụ `HealthBarCanvas` hoặc `Canvas` chính) → chuột phải → `Create Empty Child`. Đổi tên thành `SkillBarUI`.
2. Add Component → **Skill Bar UI**.
3. (Tuỳ chọn) Kéo 3 sprite icon mong muốn vào ô `Skill Icons` (0/1/2). Nếu để trống, UI sẽ hiển thị sprite mặc định (hoặc không có icon nếu `Icon Locked` cũng trống).
4. (Tuỳ chọn) Kéo 1 sprite nền vào ô `Icon Locked` để làm placeholder cho skill chưa mở.
5. Chọn lại GameObject `Player` → trong `Skill Manager` kéo `SkillBarUI` vừa tạo vào ô `Skill Bar UI`.

UI sẽ tự dựng 3 ô icon dọc bên trái khi game chạy. Khi chưa đạt level yêu cầu, ô sẽ chuyển sang màu tối (locked). Khi đang cooldown, sẽ có lớp phủ đen từ trên xuống và số giây ở giữa.

## 5. Kiểm tra Layer va chạm

Vì `SkillAOE` dùng `Physics2D.OverlapCircleAll`, enemy cần có Collider2D nằm trong Layer được query. Mặc định Unity query **mọi layer** trừ `Ignore Raycast`, nên thường chạy được. Nếu enemy nằm layer riêng (ví dụ `Enemy`), vào **Edit → Project Settings → Physics 2D → Layer Collision Matrix** đảm bảo layer `Default` (nơi AOE prefab nằm) **bật va chạm** với layer `Enemy`.

## 6. Test nhanh

1. Chạy scene.
2. Bấm phím **`** (dấu ~) để mở **Cheat Console** — nếu project có — hoặc chỉnh `currentLevel` trong `PlayerExperience` lên 3 trong Inspector khi đang Play để test mở khoá.
3. Bấm phím **1** → vùng lửa nổ quanh player, enemy trong bán kính 4 mất 30 HP.
4. Lên level 6 → bấm **2** → vùng băng + slow 2s, 50 damage.
5. Lên level 9 → bấm **3** → vùng sấm 100 damage, bán kính 7.
6. Trong khi cooldown, phím sẽ bị bỏ qua và UI hiển thị overlay + số giây còn lại.

## 7. Tuỳ chỉnh nhanh

Mở `SkillManager.cs` trong Inspector:
- `Damage`: chỉnh sát thương
- `Radius`: bán kính vùng nổ
- `Cooldown`: thời gian hồi chiêu
- `Apply Slow` + `Slow Duration` + `Slow Factor`: bậy/tắt hiệu ứng làm chậm
- `Unlock Level`: đổi mốc mở khoá (mặc định 3/6/9)
- `Key`: đổi phím tắt (mặc định 1/2/3)

## 8. Lưu ý quan trọng

- Nếu enemy có cả **Collision** lẫn **Trigger**, `OnTriggerStay2D` sẽ chạy → vẫn ăn damage từ player khi đứng cạnh. Đây là cập nhật có chủ đích để tương thích cả 2 trường hợp.
- Skill cooldown là **toàn cục** cho player, không hiện lại giữa các scene. Nếu muốn reset khi load scene mới, thêm reset vào `GameManager` (đã có sẵn `LoadMainMenu`).
- `Keyboard.current` có thể trả về `null` nếu Input System chưa khởi tạo (rất hiếm). `SkillManager` đã kiểm tra trước khi đọc.

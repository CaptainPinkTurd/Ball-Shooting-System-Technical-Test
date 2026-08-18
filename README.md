# Ball Shooting System — Technical Test

Prototype game bắn bóng 2D trên Unity: giữ chuột/ngón tay để bắn liên tục, bóng va chạm/nảy theo physics, có 2 loại bóng với behavior khác nhau (nổ delay / tách đôi khi chạm thành), tối ưu bằng object pooling để chạy ổn định với 100–200 bóng active cùng lúc.

## 1. Unity Version

- Unity **6000.3.10f1 (Unity 6 LTS)** 
- Input: **Input System package** (mới), không dùng Input Manager cũ.

## 2. Cách chạy 

1. Mở scene Test Scene lên.
2. Nhấn **Play**.
3. Test bằng chuột ngay trong Editor:
   - **Giữ chuột trái** trong vùng va chạm (`interactableZone`) của turret → bắn liên tục theo hướng từ turret tới vị trí chuột.
   - **Thả chuột** → dừng bắn.
   - Trên thiết bị thật (mobile), cùng một code path hoạt động qua touch nhờ Input System dùng chung action map cho cả Mouse và Touch.
4. Quan sát:
   - Bóng nảy qua lại giữa các bóng và tường theo physics (Rigidbody2D).
   - **Ball A**: sau khi chạm thành, đợi ~2s rồi biến mất + nổ, đẩy các bóng trong bán kính ra xa (gizmo bán kính nổ hiện khi chọn object trong Scene view).
   - **Ball B**: chạm thành là biến mất ngay, sinh ra 2 Ball A bắn ngược trở lại vào trong màn hình.

## 3. Kiến trúc (Architecture)

```
InteractableGameObject2D (abstract)          GameObjectBase (abstract)
  - lắng nghe OnStartTouch/OnEndTouch            - vòng đời Awake/OnEnable/OnDisable
  - chạy/dừng coroutine InteractionUpdate()        có event hook
  - hoạt động cho mọi input trong vùng zone      - cờ SpawnedFromPool
        │                                              │
        ▼                                              ▼
   BallTurret                                      BallBase (abstract)
   - fire rate (1/fireRate giây)                   - RequireComponent(Rigidbody2D)
   - lấy hướng bắn = target - muzzle                - OnCollisionEnter2D → lọc theo
   - lấy ball kế tiếp theo shoot order                collisionEventLayer + hasHitWall
   - spawn qua ObjectPoolManager                    - abstract OnBallCollisionEvent()
                                                     - Despawn() → trả về pool hoặc Destroy
                                                            │
                                              ┌─────────────┴─────────────┐
                                              ▼                           ▼
                                           BallA                        BallB
                                     - chờ lifeTime rồi Explode()  - bắt normal va chạm
                                     - OverlapCircleAll + AddForce - spawn N Ball A theo
                                       đẩy bóng lân cận              hướng ngược normal
                                     - spawn VFX/SFX qua pool      - Despawn() ngay

ObjectPoolManager (Singleton)                 ScreenBoundary
  - pool theo tên prefab (lookupString)          - trigger bao quanh viewport
  - SpawnObject() reuse hoặc Instantiate         - bóng lọt ra ngoài → Despawn() ngay
  - ReturnObjectToPool() → SetActive(false)        (safety net chống rò rỉ object)
```

### Danh sách script trong `Assets/_Scripts/CaptainPinkTurd/Game`

| Script | Vai trò |
|---|---|
| `BallBase.cs` | Lớp trừu tượng cho mọi loại bóng: bắt collision với tường, cờ `hasHitWall` chống bắn trùng sự kiện, `Despawn()` pool-aware. |
| `BallA.cs` | Bóng nổ trễ: đợi `lifeTime`, nổ bằng `Physics2D.OverlapCircleAll` + `AddForce` radial, phát VFX/SFX qua pool. |
| `BallB.cs` | Bóng tách đôi: bắt `contact.normal` lúc va chạm, spawn `splitBallAmount` Ball A bắn theo hướng ngược normal. |
| `BallTurret.cs` | Điều khiển bắn: kế thừa `InteractableGameObject2D`, loop theo `fireRate`, xoay theo con trỏ, quản lý `ballPrefabShootOrder`. |
| `ScreenBoundary.cs` | Trigger biên màn hình, despawn bóng lọt ra ngoài — chống leak nếu physics đẩy bóng ra khỏi khu chơi. |
| `GameManager.cs` | Boilerplate framework có sẵn (scene reset, time scale, data persistence) — không phải logic riêng của bài test này. |
| `Object Interactions/*` | `InteractableGameObject2D` (base cho input hold), `DraggableGameObject2D`, `RotatableGameObject2D` — thành phần dùng chung của framework, `BallTurret` chỉ dùng `InteractableGameObject2D`. |
| `Enemy/*`, `Player/*` | Scaffolding có sẵn từ framework cá nhân, **không liên quan** tới yêu cầu bắn bóng — giữ lại trong repo nhưng không dùng tới trong gameplay của bài test. |

### Các thành phần dùng chung (ngoài thư mục `Game`, đáng nói vì ảnh hưởng trực tiếp tới quyết định kỹ thuật)

| Script | Vai trò |
|---|---|
| `Core/Utilities/ObjectPoolManager.cs` | Pool generic theo tên prefab, singleton, tách container theo `PoolType` (GameObject/VFX/...). |
| `Core/Base/GameObjectBase.cs` | Base MonoBehaviour cho toàn bộ object gameplay, cờ `SpawnedFromPool`. |
| `Input System/Touch Input/TouchInputReader2D.cs` | Bọc Input System mới, expose `OnStartTouch`/`OnEndTouch`/`PrimaryPosition` dùng chung cho cả chuột (Editor) và touch (mobile). |

## 4. Quyết định kỹ thuật chính

### 4.1. Object Pooling thay vì Instantiate/Destroy
`ObjectPoolManager` cấp phát bóng, VFX nổ theo pool key = tên prefab. Khi bóng "biến mất" (`Despawn()`), nếu `SpawnedFromPool == true` thì được `SetActive(false)` và đưa về danh sách inactive thay vì `Destroy()`. Đây là quyết định trực tiếp phục vụ yêu cầu mục 4 (100–200 bóng active liên tục, không tăng memory, spawn/despawn nhiều lần không giảm performance) — tránh GC spike và chi phí Instantiate lặp lại.

### 4.2. Template Method pattern cho các loại bóng (Ball A/B/…)
`BallBase` xử lý phần dùng chung: lọc layer va chạm, chống bắn trùng sự kiện (`hasHitWall`), cơ chế despawn. Mỗi loại bóng chỉ cần override `OnBallCollisionEvent()` để định nghĩa behavior riêng khi chạm thành. Đây là lý do mục 5 (Extensibility) được đáp ứng: **thêm Ball Type C = tạo class mới kế thừa `BallBase`, không sửa `BallA`/`BallB`**.

### 4.3. Reset state khi tái sử dụng object (chống "state cũ bị giữ lại")
`hasHitWall` được reset về `false` trong `OnDisable()` của `BallBase` — đảm bảo khi một bóng được lấy lại từ pool, nó không mang theo trạng thái "đã chạm thành" từ lần dùng trước. Đáp ứng trực tiếp nghiệm thu mục 4 ("Không có state cũ bị giữ lại khi bóng được tạo hoặc sử dụng lại").

### 4.4. Tách "chạm thành" (bounce) khỏi "ra khỏi màn hình" (leak)
Có 2 cơ chế riêng biệt:
- **Wall colliders** (solid) — nảy vật lý bình thường qua Rigidbody2D, và là nơi trigger `OnBallCollisionEvent()` qua `OnCollisionEnter2D` lọc theo `collisionEventLayer`.
- **`ScreenBoundary`** (trigger bao ngoài) — bắt các bóng vì lý do nào đó (physics jitter, force lớn) vọt ra khỏi khu chơi, despawn ngay lập tức.

Đây là lớp an toàn thêm cho tính ổn định lifecycle ở mục 4, tách biệt rõ với logic gameplay của Ball A/B ở mục 2–3.

### 4.5. Explosion dùng `Physics2D.OverlapCircleAll` + `AddForce` radial
`BallA.Explode()` lấy tất cả collider trong `explosionRadius` (thuộc `affectedByExplosionLayers`), tính hướng từ tâm nổ ra từng object rồi `AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse)`. Lực nổ (`explosionForce`) và bán kính (`explosionRadius`) đều là field serialize — tự chọn giá trị và tinh chỉnh được trong Inspector mà không cần sửa code, phục vụ yêu cầu mục 5 (đổi radius 2→3 chỉ là đổi 1 số trong Inspector).

### 4.6. Ball B lấy hướng bật lại từ chính pháp tuyến va chạm thực tế
`BallB` override `OnCollisionEnter2D` để lưu `other.GetContact(0).normal` **trước khi** gọi `base.OnCollisionEnter2D`. Nhờ vậy hướng bắn ngược lại của 2 Ball A mới sinh ra dựa trên pháp tuyến va chạm thật (chính xác theo góc chạm tường), thay vì giả định "cứ đảo ngược velocity" — đúng hơn về mặt vật lý và đúng yêu cầu "di chuyển theo hướng ngược lại với hướng Ball B trước khi va chạm".

### 4.7. Input System dùng chung 1 action map cho Mouse và Touch
`TouchInputReader2D` bọc Input System mới với action `PrimaryContact`/`PrimaryPosition` — theo mặc định của Unity 6, action map dạng này bind cả Mouse lẫn Touchscreen vào cùng một "Primary" control. Nhờ vậy `BallTurret` (kế thừa `InteractableGameObject2D`) không cần biết đang chạy trên Editor (chuột) hay thiết bị thật (ngón tay) — cùng một code path, đáp ứng yêu cầu "có thể dùng chuột để test trên Unity Editor" mà không phải viết nhánh xử lý riêng.

### 4.8. Tách input/interaction ra khỏi turret cụ thể
`InteractableGameObject2D` là lớp base generic: lắng nghe `OnStartTouch`/`OnEndTouch`, kiểm tra bounds của `interactableZone`, chạy `InteractionUpdate()` dạng coroutine khi giữ, dừng khi thả. `BallTurret` chỉ cần implement `InteractionUpdate()` = "loop bắn theo fire rate". Tách lớp này giúp thêm cơ chế input-hold khác (nếu cần) mà không đụng vào logic bắn của turret.

## 5. Extensibility — trả lời các thay đổi mà reviewer có thể yêu cầu

| Yêu cầu thay đổi | Cách thực hiện | Có ảnh hưởng tới behaviour khác không? |
|---|---|---|
| Fire rate 2 → 5 bóng/giây | Đổi `fireRate` trên `Gun Turret` (Inspector) | Không |
| Ball A lifetime 2 → 5 giây | Đổi `lifeTime` trên prefab Ball A | Không |
| Explosion radius 2 → 3 units | Đổi `explosionRadius` trên prefab Ball A | Không |
| Ball B sinh 2 → 3 bóng | Đổi `splitBallAmount` trên prefab Ball B | Không |
| Thêm Ball Type C | Tạo class mới `BallC : BallBase`, override `OnBallCollisionEvent()`, gán prefab mới vào `ballPrefabShootOrder` của turret | Không — `BallA`/`BallB`/`BallBase` giữ nguyên |

## 6. Phần chưa hoàn thiện 

- **Chưa validate kĩ bằng Profiler thực tế trong phiên làm việc này** — kiến trúc pooling đã có sẵn để đáp ứng yêu cầu 100–200 bóng, game chạy cũng mượt trong play mode ngay cả khi tăng fire rate lên 10 bóng trên giây nhưng chưa kịp đào sâu xem có vấn đề memory leak nào khi check Profiler
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/10bba097-ae8d-4c67-82e5-9adbbb42e610" />

- **Ball Type C chưa được implement** — theo đúng phạm vi đề bài (chỉ yêu cầu kiến trúc *sẵn sàng* mở rộng), chưa tạo class cụ thể.
- **Một số script trong thư mục `Game`** (`GameManager.cs`, `Enemy/*`, `Player/*`, `DraggableGameObject2D.cs`, `RotatableGameObject2D.cs`) là scaffolding có sẵn từ framework cá nhân, không phục vụ trực tiếp gameplay bắn bóng của bài test — giữ lại vì đang nằm chung thư mục, không xóa để tránh vỡ reference khác trong project.

## 7. AI usage

README này được tổng hợp bằng cách đọc trực tiếp source code qua Unity MCP kết nối tới Unity Editor đang mở, đối chiếu với từng tiêu chí nghiệm thu trong đề bài.

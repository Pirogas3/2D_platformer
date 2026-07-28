using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PlatformCollisionComponent : MonoBehaviour
    {
        [SerializeField] private int platformLayer = 13;
        private Rigidbody2D rb;
        private bool isOnPlatform = false;
        private float collisionIgnoreTimer = 0f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            // Если номер слоя не задан, пытаемся найти по имени
            if (platformLayer == 0)
                platformLayer = LayerMask.NameToLayer("Platform (Ground)");
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && Input.GetKeyDown(KeyCode.Space))
            {
                if (isOnPlatform)
                {
                    // Отключаем коллизии между слоями
                    Physics2D.IgnoreLayerCollision(gameObject.layer, platformLayer, true);
                    // Запоминаем время, чтобы через 0.25 секунды включить обратно
                    collisionIgnoreTimer = 0.25f;
                    isOnPlatform = false; // чтобы повторно не сработало
                }
            }

            // Обновляем таймер и восстанавливаем коллизии
            if (collisionIgnoreTimer > 0)
            {
                collisionIgnoreTimer -= Time.deltaTime;
                if (collisionIgnoreTimer <= 0)
                {
                    Physics2D.IgnoreLayerCollision(gameObject.layer, platformLayer, false);
                }
            }
        }

        // Определяем, стоит ли игрок на платформе (только при касании сверху)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == platformLayer)
            {
                // Проверяем нормаль контакта, чтобы определить, что игрок сверху
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f) // нормаль направлена вверх
                    {
                        isOnPlatform = true;
                        break;
                    }
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == platformLayer)
            {
                isOnPlatform = false;
            }
        }
    }
}

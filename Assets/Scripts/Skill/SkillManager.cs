using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SkillDefinition
{
    public string skillName = "Skill";
    public int unlockLevel = 3;
    public float cooldown = 5f;
    public GameObject aoePrefab;
    public int damage = 30;
    public float radius = 4f;
    public float lifeTime = 0.3f;
    public bool applySlow = false;
    public float slowDuration = 2f;
    public float slowFactor = 0.5f;
    public Key key = Key.Digit1;
}

public class SkillManager : MonoBehaviour
{
    [Header("Skill Configs (theo thứ tự)")]
    public SkillDefinition[] skills = new SkillDefinition[]
    {
        new SkillDefinition
        {
            skillName = "Fire Burst",
            unlockLevel = 3,
            cooldown = 5f,
            damage = 30,
            radius = 4f,
            lifeTime = 0.6f,
            applySlow = false,
            key = Key.Digit1
        },
        new SkillDefinition
        {
            skillName = "Ice Nova",
            unlockLevel = 6,
            cooldown = 8f,
            damage = 50,
            radius = 5f,
            lifeTime = 0.7f,
            applySlow = true,
            slowDuration = 2f,
            slowFactor = 0.5f,
            key = Key.Digit2
        },
        new SkillDefinition
        {
            skillName = "Thunder Storm",
            unlockLevel = 9,
            cooldown = 12f,
            damage = 100,
            radius = 7f,
            lifeTime = 0.8f,
            applySlow = false,
            key = Key.Digit3
        }
    };

    [Header("References")]
    public PlayerMovement player;
    public SkillBarUI skillBarUI;

    private float[] cooldownTimers;

    void Awake()
    {
        cooldownTimers = new float[skills.Length];
    }

    void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
        }

        if (skillBarUI != null)
        {
            skillBarUI.Build(this);
        }
    }

    void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0f)
            {
                cooldownTimers[i] -= Time.deltaTime;
                if (cooldownTimers[i] < 0f) cooldownTimers[i] = 0f;
            }
        }

        for (int i = 0; i < skills.Length; i++)
        {
            SkillDefinition s = skills[i];
            if (s == null) continue;

            if (IsUnlocked(i) && Keyboard.current != null && Keyboard.current[s.key].wasPressedThisFrame)
            {
                TryCastSkill(i);
            }
        }

        if (skillBarUI != null)
        {
            skillBarUI.Refresh(this);
        }
    }

    public bool IsUnlocked(int index)
    {
        if (index < 0 || index >= skills.Length) return false;
        if (player == null) return false;
        return player.playerLevel >= skills[index].unlockLevel;
    }

    public float GetCooldownRemaining(int index)
    {
        if (index < 0 || index >= cooldownTimers.Length) return 0f;
        return cooldownTimers[index];
    }

    public float GetCooldownMax(int index)
    {
        if (index < 0 || index >= skills.Length) return 1f;
        return Mathf.Max(0.01f, skills[index].cooldown);
    }

    public void TryCastSkill(int index)
    {
        if (!IsUnlocked(index)) return;
        if (cooldownTimers[index] > 0f) return;
        if (skills[index].aoePrefab == null)
        {
            Debug.LogWarning("Skill '" + skills[index].skillName + "' chưa gán prefab AOE!");
            return;
        }

        Vector3 spawnPos = transform.position;
        GameObject aoe = Instantiate(skills[index].aoePrefab, spawnPos, Quaternion.identity);
        aoe.SetActive(false);

        SkillAOE skillAOE = aoe.GetComponent<SkillAOE>();
        if (skillAOE != null)
        {
            skillAOE.damage = skills[index].damage;
            skillAOE.radius = skills[index].radius;
            skillAOE.lifeTime = skills[index].lifeTime;
            skillAOE.applySlow = skills[index].applySlow;
            skillAOE.slowDuration = skills[index].slowDuration;
            skillAOE.slowFactor = skills[index].slowFactor;
        }

        aoe.SetActive(true);

        cooldownTimers[index] = skills[index].cooldown;
    }
}

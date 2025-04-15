using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stats is a class that is inherited by both PlayerStats and EnemyStats.
/// It is here to provide a way for Buffs to be applied to both PlayerStats
/// and EnemyStats.
/// </summary>
public abstract class EntityStats : MonoBehaviour
{
    protected float health;

    // Tinting system.
    protected SpriteRenderer sprite;
    protected Animator animator;
    protected Color originalColor;
    protected List<Color> appliedTints = new List<Color>();
    public const float TINT_FACTOR = 4f;

    [System.Serializable]
    public class Buff
    {
        public BuffData data;
        public float remainingDuration, nextTick;
        public int variant;

        public ParticleSystem effect; // Particle system associated with this buff.
        public Color tint; // Tint color associated with this buff.
        public float animationSpeed = 1f;

        public Buff(BuffData d, EntityStats owner, int variant = 0, float durationMultiplier = 1f)
        {
            data = d;
            BuffData.Stats buffStats = d.Get(variant);
            remainingDuration = buffStats.duration * durationMultiplier;
            nextTick = buffStats.tickInterval;
            this.variant = variant;

            // Save the effect so that when the debuff finishes, we can remove it.
            if (buffStats.effect) effect = Instantiate(buffStats.effect, owner.transform);
            if (buffStats.tint.a > 0)
            {
                tint = buffStats.tint;
                owner.ApplyTint(buffStats.tint);
            }

            // Apply animation speed modifications.
            animationSpeed = buffStats.animationSpeed;
            owner.ApplyAnimationMultiplier(animationSpeed);
        }

        public BuffData.Stats GetData()
        {
            return data.Get(variant);
        }
    }

    protected List<Buff> activeBuffs = new List<Buff>();

    [System.Serializable]
    public class BuffInfo
    {
        public BuffData data;
        public int variant;
        [Range(0f, 1f)] public float probability = 1f;
    }

    protected virtual void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
        animator = GetComponent<Animator>();
    }

    public virtual void ApplyAnimationMultiplier(float factor)
    {
        // Prevent factor from being zero, as it causes problems when removing.
        animator.speed *= Mathf.Approximately(0, factor) ? 0.000001f : factor;
    }

    public virtual void RemoveAnimationMultiplier(float factor)
    {
        animator.speed /= Mathf.Approximately(0, factor) ? 0.000001f : factor;
    }

    public virtual void ApplyTint(Color c)
    {
        appliedTints.Add(c);
        UpdateColor();
    }

    public virtual void RemoveTint(Color c)
    {
        appliedTints.Remove(c);
        UpdateColor();
    }

    protected virtual void UpdateColor()
    {
        // Computes the target color.
        Color targetColor = originalColor;
        float totalWeight = 1f;
        foreach(Color c in appliedTints)
        {
            targetColor = new Color(
                targetColor.r + c.r * c.a * TINT_FACTOR, 
                targetColor.g + c.g * c.a * TINT_FACTOR, 
                targetColor.b + c.b * c.a * TINT_FACTOR, 
                targetColor.a
            );
            totalWeight += c.a * TINT_FACTOR;
        }
        targetColor = new Color(
            targetColor.r / totalWeight,
            targetColor.g / totalWeight,
            targetColor.b / totalWeight,
            targetColor.a
        );

        // Set all of our sprites' colour to the computed target color.
        sprite.color = targetColor;
    }

    // Gets a certain buff from the active buffs list.
    // If <variant> is not specified, it only checks whether the buff is there.
    // Otherwise, we will only get the buff if it is the correct <data> and <variant> values.
    public virtual Buff GetBuff(BuffData data, int variant = -1)
    {
        foreach(Buff b in activeBuffs)
        {
            if (b.data == data)
            {
                // If a variant of the buff is specified, we must make
                // sure our buff is the same variant before returning it.
                if (variant >= 0)
                {
                    if (b.variant == variant) return b;
                }
                else
                {
                    return b;
                }
            }
        }
        return null;
    }

    // If applying a buff via BuffInfo, we will also check the probability first.
    public virtual bool ApplyBuff(BuffInfo info, float durationMultiplier = 1f)
    {
        if(Random.value <= info.probability)
            return ApplyBuff(info.data, info.variant, durationMultiplier);
        return false;
    }

    // Adds a buff to an entity.
    public virtual bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1f)
    {
        Buff b;
        BuffData.Stats s = data.Get(variant);

        switch(s.stackType)
        {
            // If the buff stacks fully, then we can have multiple copies of the buff.
            case BuffData.StackType.stacksFully:
                activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                RecalculateStats();
                return true;

            // If it only refreshes the duration, we will find the buff that is
            // already there and reset the remaining duration (if the buff is not there yet).
            // Otherwise, we just add the buff.
            case BuffData.StackType.refreshDurationOnly:
                b = GetBuff(data, variant);
                if(b != null)
                {
                    b.remainingDuration = s.duration * durationMultiplier;
                } 
                else
                {
                    activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                    RecalculateStats();
                }
                return true;

            // In cases where buffs do not stack, if the buff already exists, we ignore it.
            case BuffData.StackType.doesNotStack:
                b = GetBuff(data, variant);
                if (b != null)
                {
                    activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                    RecalculateStats();
                    return true;
                }
                return false;
        }
        
        return false;
    }

    // Removes all copies of a certain buff.
    public virtual bool RemoveBuff(BuffData data, int variant = -1)
    {
        // Loop through all the buffs, and find buffs that we need to remove.
        List<Buff> toRemove = new List<Buff>();
        foreach (Buff b in activeBuffs)
        {
            if (b.data == data)
            {

                if (variant >= 0)
                {
                    if (b.variant == variant) toRemove.Add(b);
                }
                else
                {
                    toRemove.Add(b);
                }
            }
        }

        // We need to remove the buffs outside of the loop, otherwise this
        // will cause performance issues with the foreach loop above.
        if(toRemove.Count > 0)
        {
            foreach(Buff b in toRemove)
            {
                if (b.effect) Destroy(b.effect.gameObject);
                if (b.tint.a > 0) RemoveTint(b.tint);
                RemoveAnimationMultiplier(b.animationSpeed);
                activeBuffs.Remove(b);
            }
            RecalculateStats();
            return true;
        }
        return false;
    }

    // Generic take damage function for dealing damage.
    public abstract void TakeDamage(float dmg);

    // Generic restore health function.
    public abstract void RestoreHealth(float amount);

    // Generic kill function.
    public abstract void Kill();

    // Forces the entity to recalculate its stats.
    public abstract void RecalculateStats();

    protected virtual void Update()
    {
        // Counts down each buff and removes them after their remaining
        // duration falls below 0.
        List<Buff> expired = new List<Buff>();
        foreach(Buff b in activeBuffs)
        {
            BuffData.Stats s = b.data.Get(b.variant);

            // Tick down on the damage / heal timer.
            b.nextTick -= Time.deltaTime;
            if (b.nextTick < 0)
            {
                float tickDmg = b.data.GetTickDamage(b.variant);
                if(tickDmg > 0) TakeDamage(tickDmg);
                float tickHeal = b.data.GetTickHeal(b.variant);
                if(tickHeal > 0) RestoreHealth(tickHeal);
                b.nextTick = s.tickInterval;
            }

            // If the buff has a duration of 0 or less, it will stay forever.
            // Don't reduce the remaining duration.
            if (s.duration <= 0) continue;

            // Also tick down on the remaining buff duration.
            b.remainingDuration -= Time.deltaTime;
            if (b.remainingDuration < 0) expired.Add(b);
        }

        // We remove the buffs outside the foreach loop, as it will affect the
        // iteration if we remove items from the list while a loop is still running.
        foreach (Buff b in expired)
        {
            if (b.effect) Destroy(b.effect.gameObject);
            if (b.tint.a > 0) RemoveTint(b.tint);
            RemoveAnimationMultiplier(b.animationSpeed);
            activeBuffs.Remove(b);
        }
        RecalculateStats();
    }
}
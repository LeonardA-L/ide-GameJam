using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class AnimatorsManager
    {
        private static AnimatorsManager m_instance;
        public static AnimatorsManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        private List<Animator> m_animators = new List<Animator>();
        private List<ParticleSystem> m_particles = new List<ParticleSystem>();
        private bool m_paused = false;

        public AnimatorsManager()
        {
            m_instance = this;
        }

        public void Play()
        {
            m_animators.RemoveAll(item => item == null);

            foreach (Animator anim in m_animators)
            {
                anim.enabled = true;
            }

            foreach (ParticleSystem particle in m_particles)
            {
                particle.Play();
            }

            m_paused = false;
        }

        // Animators

        public void RegisterAnimator(Animator _animator)
        {
            if (m_paused)
                _animator.enabled = false;
            m_animators.Add(_animator);
        }

        public void UnregisterAnimator(Animator _animator)
        {
            m_animators.Remove(_animator);
        }

        public void Pause()
        {
            foreach (Animator anim in m_animators)
            {
                anim.enabled = false;
            }

            foreach (ParticleSystem particle in m_particles)
            {
                particle.Pause();
            }
            m_paused = true;
        }

        // Particles

        public void RegisterParticleSystem(ParticleSystem _system)
        {
            if (m_paused)
                _system.Pause();

            m_particles.Add(_system);
        }
    }
}
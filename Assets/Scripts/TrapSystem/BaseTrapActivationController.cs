﻿using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class BaseTrapActivationController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Button Animation Properties")]
        [Tooltip("The trap button object to be used.")]
        [SerializeField] private GameObject trapButtonObject;
        [Tooltip("The trap button's position and rotation when ready to be pressed.")]
        [SerializeField] private Transform trapButtonReadyTransform;
        [Tooltip("The trap button's position offset when pressed.")]
        [SerializeField] private Vector3 trapButtonPressedOffset;
        [Tooltip("The trap button's enable/disable animation duration.")]
        [SerializeField] private float trapButtonAnimationDuration = 1f;

        private Vector3 trapButtonReadyPosition;
        private Vector3 trapButtonPressedPosition;

        [Header("Trap Activation Properties")]
        [Tooltip("The trap effect to be activated.")]
        [SerializeField] private BaseTrapEffectController trapEffectController;
        [Tooltip("The delay before activating a trap.")]
        [SerializeField] float trapActivationDelay;
        [Tooltip("The trap activator's cooldown.")]
        [SerializeField] private float trapActivationCooldown;

        private bool trapActivatorOnCooldown = false;
        #endregion

        #region Unity Methods
        void Start()
        {
            trapButtonReadyPosition = trapButtonReadyTransform.position;
            trapButtonPressedPosition = trapButtonReadyPosition + trapButtonPressedOffset;
        }

        virtual protected void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player") && !trapActivatorOnCooldown)
            {
                StartCoroutine(TriggerTrapActivation());
            }
        }
        #endregion

        #region Custom Methods
        private void AnimatePressed()
        {
            StartCoroutine(AnimateButtonMovement(trapButtonReadyPosition, trapButtonPressedPosition));
        }

        private void AnimateReady()
        {
            StartCoroutine(AnimateButtonMovement(trapButtonPressedPosition, trapButtonReadyPosition));
        }
        #endregion

        #region Coroutines
        private IEnumerator TriggerTrapActivation()
        {
            AnimatePressed();

            yield return new WaitForSeconds(trapActivationDelay);

            trapEffectController.ActivateTrap();
            trapActivatorOnCooldown = true;

            yield return new WaitForSeconds(trapActivationCooldown - trapActivationDelay);

            trapActivatorOnCooldown = false;
            AnimateReady();
        }

        private IEnumerator AnimateButtonMovement(Vector3 initialPosition, Vector3 finalPosition)
        {
            float t = 0f;
            while (t <= 1f)
            {
                trapButtonObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

                float tIncrease = Time.deltaTime / trapButtonAnimationDuration;

                t += tIncrease;

                yield return new WaitForSeconds(tIncrease);
            }
        }
        #endregion
    }
}

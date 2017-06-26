using UnityEngine;
using UnityEngine.UI;
using RailShooter.Utils;

namespace RailShooter.Utils
{
    // This is a simple script to tint UI Images a colour when
    // a InteractiveItem is looked at.  Note that the Images
    // to tint don't have to be on the same gameobject as the
    // InteractiveItem but the user will be looking at the
    // InteractiveItem when the Images are tinted.
    // Also note that this should be used to tint/highlight
    // images, not change their colour entirely.

    public class UITint : MonoBehaviour
    {
        [SerializeField] private Color m_Tint;                                  // The colour to tint the Images.
        [Range (0f, 1f)] [SerializeField] private float m_TintPercent = 0.5f;   // How much the colour should be used.
        [SerializeField] private Image[] m_ImagesToTint;                        // References to the images which will be tinted.
        [SerializeField] private InteractiveItem m_InteractiveItem;           // Reference to the InteractiveItem which must be looked at to tint the images.


        private void OnEnable ()
        {
            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;
        }


        private void OnDisable ()
        {
            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;
        }


        private void HandleOver ()
        {
            // When the user looks at the InteractiveItem go through all the images...
            for (int i = 0; i < m_ImagesToTint.Length; i++)
            {
                m_ImagesToTint[i].color += m_Tint * m_TintPercent;
            }
        }


        private void HandleOut ()
        {
            // When the user looks away from the InteractiveItem go through all the images...
            for (int i = 0; i < m_ImagesToTint.Length; i++)
            {
                m_ImagesToTint[i].color -= m_Tint * m_TintPercent;
            }
        }
    }
}
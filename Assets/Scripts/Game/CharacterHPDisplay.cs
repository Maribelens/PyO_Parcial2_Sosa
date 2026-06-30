using TMPro;
using UnityEngine;
using RPGCombat.Characters;

namespace RPGCombat.UI
{
    // SRP: solo muestra el HP de UN personaje. No decide nada de combate ni turnos.
    // Se asigna manualmente en el Inspector a cada personaje de la escena.
    public class CharacterHPDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private MonoBehaviour characterSource; // debe implementar ICharacter

        private ICharacter character;

        private void Awake()
        {
            character = characterSource as ICharacter;
        }

        public void Refresh()
        {
            if (character == null) return;

            label.text = character.IsAlive
                ? $"{character.CharacterName}\nHP: {character.CurrentHP}/{character.MaxHP}"
                : $"{character.CharacterName}\n(caído)";

            label.color = character.IsAlive ? Color.white : Color.gray;
        }
    }
}

#RPG Combat — 2° Examen Parcial
**Materia:** Portabilidad y Optimización

### Scriptable Objects para datos de personaje

Los stats de cada personaje (HP, velocidad, daños, curación) están encapsulados en
assets `CharacterData` independientes del código. Esto permite:
- Modificar el balance del juego sin tocar ningún script.
- Reutilizar el mismo prefab base con distintos datos.
- Cumple con el principio DRY: cada valor existe en un único lugar.

---

### Clean Code

- **Nombres descriptivos:** `CanMeleeAttack`, `TryRangeAttack`, `GetNearestPlayer`,
  `HasMovedThisTurn` son autoexplicativos sin necesidad de comentarios adicionales.
- **Sin valores hardcodeados:** todas las constantes de stats viven en los Scriptable
  Objects. Las constantes de lógica (`HealMaxDistance`, etc.) están declaradas con
  nombre semántico en cada clase.
- **Sin ifs anidados:** la selección de tipo de acción usa pattern matching
  (`switch` expression). La selección de objetivos usa LINQ con condiciones claras.
- **DRY:** `ChebyshevDistance` y `ManhattanDistance` están una sola vez en
  `Character` y se reutilizan en todas las subclases y en `EnemyAI`.
- **YAGNI:** no se implementó nada más allá de lo especificado en el enunciado.
- **KISS:** el sistema de turnos es una máquina de estados simple con 3 fases
  (`Movement → Action → Done`), sin sobreingeniería.
- **Legible antes que conciso:** se priorizó la claridad del código sobre la brevedad.
  Por ejemplo, `GetAvailableActions()` construye la lista explícitamente en vez de
  una sola expresión LINQ comprimida.
  
---

### Optimización de assets

- Grilla generada proceduralmente: un único `cellPrefab` instanciado 24 veces,
  sin duplicar assets ni texturas.
- Personajes instanciados en runtime por `GameInitializer`, no pre-colocados en
  escena, para mantener la escena limpia y el spawn aleatorio correcto.
- Los Scriptable Objects son assets compartidos: múltiples instancias del mismo
  tipo de enemigo referencian el mismo `Enemy_Data.asset` sin duplicar datos.

---

### Consideraciones de escena

- La cámara es ortográfica 2D. `GridToWorld` usa ejes X e Y (`new Vector3(x, y, 0f)`)
  para que las filas sean visibles en pantalla.
- Los HP displays de la UI se conectan con los personajes instanciados en runtime
  a través de `GameInitializer.ConnectHPDisplays()`, evitando referencias a prefabs
  en disco que tendrían HP = 0.
- El Input System está configurado en modo `Both` para compatibilidad con
  `UnityEngine.Input.GetKeyDown` (Input Manager clásico).


---


## Optimización de assets y escena

### Sprite Atlas

Se crearon dos Sprite Atlas para reducir draw calls por sprites:

- `Atlas_Characters` — agrupa los 4 sprites de personajes (Fighter, Healer, Ranger, Enemy × 2). Todos los personajes aparecen en pantalla al mismo tiempo, por lo que conviene agruparlos en un mismo atlas.
- `Atlas_Grid` — agrupa el sprite de la celda de pasto por separado, ya que es un asset que nunca aparece "junto" a un personaje muerto (los personajes desaparecen), y el PDF recomienda no mezclar en un mismo atlas assets que nunca van a aparecer simultáneamente.

Resultado: de 6 draw calls por sprites se pasa a 2 (uno por atlas).

**Configuración aplicada en cada sprite antes de empacar:**
- `Max Size: 512` — suficiente para el tamaño en pantalla de una grilla 4×6
- `Compression: Normal Quality`
- `Generate Physics Shape: desactivado` — los personajes se mueven por grilla con código, sin física
- `Tight Packing: desactivado` — los sprites usan Mesh Type: Tight por defecto

---

### Canvas dividido en dos

Cuando cualquier elemento de un Canvas cambia, Unity redibuja todo ese Canvas. El `HPPanel` se actualiza en cada acción (potencialmente varias veces por turno), mientras que el `GameOverPanel` solo se activa una vez al final de la partida. Mezclarlos en el mismo Canvas haría que el panel de game over force un redibujado innecesario en cada actualización de HP.

Solución: dos Canvas separados:
- `Canvas_Game` — contiene TurnPanel, ActionButtons, TargetButtons y HPPanel. Se redibuja frecuentemente.
- `Canvas_Overlay` — contiene solo GameOverPanel. Permanece inactivo casi toda la partida.

---

### Raycast Target desactivado en textos

Por defecto, todos los componentes `TMP_Text` tienen `Raycast Target` activado, lo que hace que el EventSystem de Unity chequee colisiones contra ellos en cada frame aunque no sean interactivos. Se desactivó `Raycast Target` en todos los textos que no reciben input: `ActiveCharacterLabel`, `PhaseLabel`, todos los HP labels, `GameOverLabel` y los textos hijos de los botones de objetivo.

Solo los `Button` conservan `Raycast Target` activo, que es el único componente que lo necesita.

---

### Compresión de texturas

Todos los sprites del proyecto fueron configurados con:
- `Max Size: 512` — apropiado para el contexto visual del juego (grilla pequeña en pantalla)
- `Resize Algorithm: Mitchell` (default) — balance óptimo entre calidad y velocidad de compresión
- `Compression: Normal Quality`
- `Generate Physics Shape: desactivado` — ningún sprite del proyecto requiere detección de colisiones por física

---

### Frame Rate limitado a 60 FPS

Al ser un juego por turnos, no hay ninguna ventaja en correr a más de 60 FPS. Se limita el frame rate en `GameFlowController.Start()`:

```csharp
Application.targetFrameRate = 60;
QualitySettings.vSyncCount = 0;
```

Esto reduce el uso innecesario de CPU/GPU en los frames donde el juego simplemente espera el input del jugador, que representan la mayor parte del tiempo de ejecución en un juego por turnos.
---

### Optimizaciones de UI (Canvas) aplicadas

- **Sin Layout Groups:** los elementos de UI están posicionados con anchors y RectTransform fijos, no con Layout Groups. Cada Layout Group hace un `GetComponent` de todos sus subelementos cuando cualquier hijo cambia, lo cual es costoso.

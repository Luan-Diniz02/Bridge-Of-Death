# 🎮 Sistema PlayerManager - Guia de Configuração

## ✅ O Que Foi Implementado

### Novos Scripts:
1. **GameSettings.cs** - Configurações globais (ScriptableObject)
2. **PlayerManager.cs** - Gerenciador de players (spawn e referências)

### Scripts Atualizados:
1. **PlayerController.cs** - Adicionado Singleton
2. **EnemyController.cs** - Usa PlayerManager ao invés de referência manual
3. **ObstacleController.cs** - Detecta player automaticamente
4. **DistanceView.cs** - Busca player via PlayerManager

---

## 🚀 Como Configurar

### Passo 1: Criar GameSettings (ScriptableObject)

1. No Unity, crie uma pasta `Resources` em `Assets` (se não existir)
2. Clique direito em `Assets/Resources`
3. **Create > Game > Settings**
4. Renomeie para **"GameSettings"** (nome exato!)
5. Configure:
   - **Is Two Player Mode**: ☐ (desmarque para single player)
   - **Split Screen Enabled**: ☐
   - **Max Players**: 2

### Passo 2: Configurar PlayerManager na Cena de Gameplay

1. Na sua cena de gameplay, crie um GameObject vazio
2. Renomeie para **"PlayerManager"**
3. Adicione o component **PlayerManager**
4. Configure:
   - **Spawn Points**: 
     - Size: 2 (ou quantos players você quer suportar)
     - Element 0: Crie um Empty GameObject na posição inicial do Player 1
     - Element 1: Crie um Empty GameObject na posição inicial do Player 2
   - **Spawn On Start**: ✓ (marcado)
   - **Default Player Prefab**: Arraste o prefab padrão do player (fallback)

### Passo 3: Remover Player Manual da Cena

⚠️ **IMPORTANTE**: Se você tinha um player já na cena:
1. **Delete o GameObject do player** da hierarquia
2. O PlayerManager irá instanciar automaticamente no Play

### Passo 4: Criar Prefabs dos Personagens

Cada personagem que você criou para a loja precisa ser um **Prefab**:

1. Para cada modelo de personagem:
   - Crie um GameObject com o modelo 3D
   - Adicione **PlayerController** component
   - Adicione todos os components necessários:
     - CharacterController
     - Animator
     - PlayerInput
     - CameraController
     - LaneController
     - AudioSource
   - Configure tudo igual ao player antigo
   - **Arraste para a pasta Prefabs** para criar o prefab
   - Delete da hierarquia

2. No **SimpleStoreCharacter** de cada personagem na loja:
   - Arraste o prefab correspondente para **Character Prefab**

### Passo 5: Configurar Tags

Certifique-se que existe a tag:
- **Player** (já deve existir)
- Opcionalmente crie **Player2** para multiplayer

---

## 🎯 Como Funciona Agora

### Single Player (1 Jogador)
```
GameSettings.IsTwoPlayerMode = false
PlayerManager spawna 1 player no Spawn Point [0]
```

### Multiplayer (2 Jogadores)
```
GameSettings.IsTwoPlayerMode = true
PlayerManager spawna 2 players:
  - Player 1 no Spawn Point [0]
  - Player 2 no Spawn Point [1]
```

---

## 📋 Referências ao Player nos Scripts

### Antes (Manual):
```csharp
[SerializeField] private GameObject player; // ❌
```

### Agora (Automático):

#### Para encontrar o player mais próximo (Inimigos):
```csharp
PlayerController player = PlayerManager.Instance.GetClosestPlayer(transform.position);
```

#### Para player específico (Câmera, UI):
```csharp
PlayerController player1 = PlayerManager.Instance.GetPlayer(0); // Player 1
PlayerController player2 = PlayerManager.Instance.GetPlayer(1); // Player 2
```

#### Para detectar colisão (já funciona):
```csharp
void OnTriggerEnter(Collider other)
{
    PlayerController player = other.GetComponent<PlayerController>();
    if (player != null)
    {
        // Faz algo com o player
    }
}
```

---

## 🔧 Scripts Atualizados - O Que Mudou

### EnemyController.cs
- ❌ Removido `[SerializeField] private GameObject player`
- ✅ Busca automaticamente o player mais próximo via PlayerManager
- ✅ Suporta múltiplos players (persegue o mais próximo)

### ObstacleController.cs
- ❌ Removido `[SerializeField] private PlayerController playerController`
- ✅ Detecta automaticamente qual player colidiu
- ✅ Funciona com Player 1 e Player 2

### DistanceView.cs
- ❌ Removido `[SerializeField] private Transform playerTransform`
- ✅ Adicionado `[SerializeField] private int playerIndex` (qual player seguir)
- ✅ Busca automaticamente via PlayerManager
- ✅ Suporta UI separada para cada player

### PlayerController.cs
- ✅ Adicionado Singleton: `PlayerController.Instance`
- ✅ Útil para acesso rápido em single player
- ⚠️ Em multiplayer, use `PlayerManager.GetPlayer(index)`

---

## 🎮 Preparado para Multiplayer/Tela Dividida

O sistema já está preparado para quando você implementar:

### Para ativar 2 jogadores:
1. No **GameSettings**, marque `Is Two Player Mode = true`
2. Pronto! O PlayerManager spawna 2 players automaticamente

### Câmeras separadas (Split Screen):
```csharp
// Configurar 2 câmeras, uma para cada player
Camera cam1 = camera1Object.GetComponent<Camera>();
Camera cam2 = camera2Object.GetComponent<Camera>();

// Player 1: metade esquerda da tela
cam1.rect = new Rect(0, 0, 0.5f, 1);

// Player 2: metade direita da tela
cam2.rect = new Rect(0.5f, 0, 0.5f, 1);

// Fazer cada câmera seguir seu player
cam1.GetComponent<CameraController>().SetTarget(PlayerManager.Instance.GetPlayer(0));
cam2.GetComponent<CameraController>().SetTarget(PlayerManager.Instance.GetPlayer(1));
```

---

## ⚠️ Checklist Final

- [ ] GameSettings criado em `Assets/Resources/GameSettings`
- [ ] PlayerManager adicionado na cena de gameplay
- [ ] Spawn Points configurados (mínimo 2)
- [ ] Player removido manualmente da cena
- [ ] Personagens convertidos em Prefabs
- [ ] Prefabs configurados no SimpleStoreCharacter
- [ ] Testar no Play Mode - player deve spawnar automaticamente
- [ ] Verificar que inimigos perseguem o player
- [ ] Verificar que obstáculos causam dano
- [ ] Verificar que UI de distância funciona

---

## 🐛 Troubleshooting

**Player não spawna:**
- Verifique se PlayerManager está na cena
- Confirme que Spawn Points estão configurados
- Verifique se tem um personagem selecionado na loja OU Default Player Prefab configurado

**Inimigos não seguem o player:**
- Certifique-se que PlayerManager spawnou o player primeiro
- Verifique no console se há warnings

**"PlayerManager não encontrado":**
- PlayerManager deve estar ATIVO na cena
- Não pode estar desabilitado

**Personagem da loja não aparece:**
- Verifique se o Character Prefab está configurado no SimpleStoreCharacter
- Certifique-se que tem um personagem selecionado (IsDefault = true para um)

---

## 🎯 Próximos Passos Sugeridos

1. ✅ Testar sistema em single player
2. ✅ Configurar prefabs de todos os personagens
3. ⏳ Implementar split screen para multiplayer
4. ⏳ Configurar input separado (Keyboard vs Gamepad)
5. ⏳ UI separada para cada player (health, score, etc)

---

**Sistema pronto e preparado para futuro multiplayer!** 🚀

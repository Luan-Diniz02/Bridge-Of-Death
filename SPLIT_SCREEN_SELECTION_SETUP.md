# Sistema de Seleção de Personagens - Split Screen

## Visão Geral
Sistema que permite selecionar um personagem principal na loja e um segundo personagem quando o jogador escolhe o modo Split Screen.

## Fluxo do Sistema

### Single Player
1. Jogador seleciona personagem na **Loja**
2. Jogador clica em **Play → Single Player**
3. `CharacterSelection` carrega apenas Player 1
4. `PlayerManager` spawna 1 player

### Split Screen
1. Jogador seleciona **primeiro personagem** na **Loja** (Player 1)
2. Jogador clica em **Play → Split Screen**
3. Sistema abre interface para selecionar **segundo personagem** (Player 2)
4. Jogador seleciona Player 2
5. `CharacterSelection` salva ambos os personagens
6. `PlayerManager` spawna 2 players com personagens diferentes

## Arquivos Criados/Modificados

### 1. `CharacterSelection.cs`
**Modificações:**
- Adicionado suporte para Player 2
- Métodos novos:
  - `SetPlayer2Character(string id, GameObject prefab)` - Define personagem do Player 2
  - `GetPlayer2CharacterPrefab()` - Obtém prefab do Player 2
  - `HasPlayer2Selection()` - Verifica se Player 2 tem seleção
  - `ClearPlayer2Selection()` - Limpa seleção do Player 2

### 2. `PlayerSelectionMenu.cs` (NOVO)
Gerencia a tela de seleção de modo de jogo.

**Métodos principais:**
- `OnSinglePlayerClicked()` - Inicia jogo em modo single player
- `OnSplitScreenClicked()` - Abre seleção do segundo personagem
- `OnPlayer2Selected()` - Confirma seleção do Player 2

### 3. `PlayerManager.cs`
**Modificações:**
- `GetPlayerPrefab(playerIndex)` agora verifica se é Player 2
- Se Player 2 e tem seleção específica → usa `CharacterSelection.GetPlayer2CharacterPrefab()`
- Se Player 2 mas sem seleção → usa mesmo prefab do Player 1

### 4. `SimpleStoreManager.cs`
**Correções:**
- Agora desmarca TODOS os personagens antes de carregar seleção
- Garante que apenas 1 personagem seja marcado como "SELECIONADO"

### 5. `CurrencyManager.cs`
**Correções:**
- Limpa eventos no `OnDestroy()` para evitar warnings de limpeza

## Como Implementar na UI

### Passo 1: Menu Play
Na cena **Menu**, crie a hierarquia:

```
Canvas
└── PlayMenu (GameObject)
    ├── SinglePlayerButton (Button)
    └── SplitScreenButton (Button)
```

1. Adicione o script **PlayerSelectionMenu** ao `PlayMenu`
2. Configure no Inspector:
   - **Store Scene Name**: "Menu" (ou nome da sua cena de loja)
   - **Gameplay Scene Name**: "Single Player" (ou nome da cena de jogo)
3. Conecte os botões:
   - `SinglePlayerButton.onClick` → `PlayerSelectionMenu.OnSinglePlayerClicked()`
   - `SplitScreenButton.onClick` → `PlayerSelectionMenu.OnSplitScreenClicked()`

### Passo 2: Seleção do Player 2

**Opção A - Painel Simples (Recomendado)**

Crie um painel na mesma cena do menu:

```
Canvas
└── Player2SelectionPanel (GameObject - desativado por padrão)
    ├── Title (Text: "Selecione o Player 2")
    ├── Character1Button
    ├── Character2Button
    └── Character3Button
```

1. Cada botão de personagem deve chamar um método que:
```csharp
public void SelectPlayer2Character(string characterID)
{
    // Encontra o personagem na loja pelo ID
    SimpleStoreManager storeManager = FindFirstObjectByType<SimpleStoreManager>();
    var character = storeManager.GetCharacterByID(characterID);
    
    if (character != null && character.IsPurchased)
    {
        // Salva Player 2
        CharacterSelection.Instance.SetPlayer2Character(
            character.CharacterID, 
            character.CharacterPrefab
        );
        
        // Carrega gameplay
        SceneManager.LoadScene("Single Player");
    }
}
```

**Opção B - Reutilizar Loja**

Use a mesma interface da loja mas em modo "Player 2":

1. Adicione flag temporária quando clicar em Split Screen:
```csharp
PlayerPrefs.SetInt("SelectingPlayer2", 1);
SceneManager.LoadScene("Menu"); // Volta para loja
```

2. Na loja, detecte o modo:
```csharp
if (PlayerPrefs.GetInt("SelectingPlayer2", 0) == 1)
{
    // Muda título para "Selecione o Player 2"
    // Ao clicar em personagem, salva como Player 2
}
```

### Passo 3: Verificar Inspector da Loja

⚠️ **IMPORTANTE**: Na cena da **Loja**, verifique cada `SimpleStoreCharacter`:

1. Abra a hierarquia da loja
2. Para cada personagem (Luan, Frank, Saulo):
   - Selecione o GameObject
   - No Inspector, procure o componente `SimpleStoreCharacter`
   - **Apenas UM personagem** deve ter `Is Default = ✓ (marcado)`
   - Os outros devem ter `Is Default = ☐ (desmarcado)`

Exemplo correto:
```
Luan:  Is Default = ✓   (Personagem inicial)
Frank: Is Default = ☐
Saulo: Is Default = ☐
```

## Fluxo de Dados

### Single Player
```
Loja → SimpleStoreManager.SelectCharacter()
     → CharacterSelection.SetSelectedCharacter() (Player 1)
     → PlayerPrefs["SelectedCharacter"] = "Luan"
     
Gameplay → PlayerManager.SpawnPlayers()
         → GetPlayerPrefab(0) // Player 1
         → CharacterSelection.GetSelectedCharacterPrefab()
         → Spawna "Luan"
```

### Split Screen
```
Loja → SimpleStoreManager.SelectCharacter()
     → CharacterSelection.SetSelectedCharacter() (Player 1)
     → PlayerPrefs["SelectedCharacter"] = "Luan"
     
Menu Play → OnSplitScreenClicked()
          → Abre seleção Player 2
          → Jogador escolhe "Frank"
          → CharacterSelection.SetPlayer2Character("Frank", prefab)
          → PlayerPrefs["SelectedCharacter_Player2"] = "Frank"
          
Gameplay → PlayerManager.SpawnPlayers()
         → GetPlayerPrefab(0) → Spawna "Luan" (Player 1)
         → GetPlayerPrefab(1) → Spawna "Frank" (Player 2)
```

## Debugging

### Console Logs
O sistema gera logs detalhados:

```
LoadSelectedCharacter: savedID = 'Luan'
Encontrado personagem salvo: Luan
SimpleStoreManager: Personagem Luan selecionado e salvo!
Player 1: Usando personagem do CharacterSelection: Luan
Player 2: Usando personagem do CharacterSelection: Frank
```

### Problema: Múltiplos "SELECIONADO"
- **Causa**: Mais de um personagem com `Is Default = true` no Inspector
- **Solução**: Verifique todos os `SimpleStoreCharacter` e deixe apenas 1 com `Is Default = true`

### Problema: Warning do CurrencyManager
- **Status**: ✅ Corrigido - eventos são limpos no `OnDestroy()`

## PlayerPrefs Usados

| Key | Valor | Descrição |
|-----|-------|-----------|
| `SelectedCharacter` | "Luan", "Frank", "Saulo" | ID do personagem do Player 1 |
| `SelectedCharacter_Player2` | "Luan", "Frank", "Saulo" | ID do personagem do Player 2 |
| `Character_Purchased_Luan` | 0 ou 1 | Se personagem foi comprado |
| `SelectingPlayer2` | 0 ou 1 | Flag temporária: está selecionando Player 2 |

## Próximos Passos

1. ✅ Verificar Inspector: apenas 1 personagem com `Is Default = true`
2. ⏳ Criar UI do menu de seleção (Single Player / Split Screen)
3. ⏳ Implementar painel de seleção do Player 2
4. ⏳ Testar fluxo completo
5. ⏳ Integrar com UIManager para barras de power-ups

## Notas Importantes

- O Player 1 é SEMPRE selecionado na loja
- O Player 2 é selecionado APENAS quando clicar em Split Screen
- Se não houver seleção de Player 2, ele usa o mesmo prefab do Player 1
- Quando voltar para Single Player, limpe a seleção do Player 2:
```csharp
CharacterSelection.Instance.ClearPlayer2Selection();
```

# Sistema de Loja Simplificado - Bridge Of Death

## 📋 Visão Geral

Sistema simples de loja para compra de personagens usando moedas coletadas durante o jogo. Focado em facilidade de uso com personagens já instanciados na UI.

## 🎯 Arquivos Criados

### 1. **CurrencyManager.cs** (Singleton)
Gerencia as moedas do jogador de forma persistente:
- Adicionar/gastar moedas
- Salvar/carregar usando PlayerPrefs
- Sistema de eventos para atualização de UI

### 2. **SimpleStoreCharacter.cs**
Script que você anexa a cada personagem na sua UI da loja:
- Lógica de compra e seleção
- Gerenciamento de estado (comprado/selecionado)
- Atualização automática de UI
- Persistência individual

### 3. **SimpleStoreManager.cs**
Gerenciador simples que coordena todos os personagens:
- Controla qual personagem está selecionado
- Atualiza display de moedas
- Carrega estado salvo

## 🚀 Como Usar

### Passo 1: Configurar o Painel da Loja

1. **No seu Canvas**, crie/abra o painel da loja que você já tem
2. **Adicione o componente `SimpleStoreManager`** ao GameObject principal da loja
3. **Configure a referência:**
   - **Currency Text**: Arraste o TextMeshProUGUI que mostra as moedas

### Passo 2: Configurar Cada Personagem

Para cada personagem já instanciado na sua UI:

1. **Adicione o componente `SimpleStoreCharacter`**
2. **Configure no Inspector:**
   - **Character ID**: ID único (ex: "character_1", "character_2")
   - **Price**: Preço em moedas (ex: 50)
   - **Is Default**: ✓ apenas para o personagem inicial gratuito
   
3. **Arraste as referências UI:**
   - **Action Button**: Botão único (comprar/selecionar)
   - **Button Text**: TextMeshProUGUI do texto do botão
   - **Coin Icon**: GameObject do ícone da moeda (aparece só quando não comprado)
   - **Locked Overlay**: (Opcional) Overlay quando bloqueado
   - **Selected Indicator**: (Opcional) Indicador visual de selecionado
   
**Como o botão funciona:**
- **Não comprado**: Mostra o preço + ícone da moeda
- **Comprado**: Mostra "SELECIONAR"
- **Selecionado**: Mostra "SELECIONADO" (desabilitado)

### Passo 3: Pronto!

O sistema funciona automaticamente:
- ✅ Moedas são salvas ao completar fases
- ✅ Compras são salvas automaticamente
- ✅ Personagem selecionado é lembrado
- ✅ UI atualiza automaticamente

## 💰 Sistema de Moedas

### Adicionar Moedas

O sistema já está integrado com o `LevelCompleted.cs`. Ao completar uma fase, as moedas são automaticamente adicionadas ao total persistente:

```csharp
// Já implementado em LevelCompleted.cs
CurrencyManager.Instance.AddCurrency(totalCoins);
```

### Verificar Moedas Atuais

```csharp
int moedas = CurrencyManager.Instance.GetCurrentCurrency();
```

### Gastar Moedas

```csharp
bool sucesso = CurrencyManager.Instance.SpendCurrency(100);
```

## 🎮 Selecionando Personagem no Jogo

Para usar o personagem selecionado no jogo:

```csharp
// Obter o personagem selecionado
CharacterData selectedChar = storeManager.GetSelectedCharacterData();

if (selectedChar != null)
{
    // Instanciar o prefab do personagem
    GameObject player = Instantiate(selectedChar.CharacterPrefab);
}
```

## 🛠️ Funções de Debug

O StoreManager possui funções de debug acessíveis pelo menu de contexto:

- **Unlock All Characters**: Desbloqueia todos os personagens
- **Reset All Purchases**: Reseta todas as compras

Para usar: Clique com botão direito no componente StoreManager no Inspector.

## 📊 Sistema de Eventos

O sistema usa eventos para notificação:

```csharp
// Escutar mudanças de moeda
CurrencyManager.Instance.OnCurrencyChanged += (amount) => 
{
    Debug.Log($"Moedas: {amount}");
};

// Escutar compras
storeManager.OnItemPurchased += (item) => 
{
    Debug.Log($"Comprado: {item.CharacterData.CharacterName}");
};

// Escutar seleção de personagem
storeManager.OnCharacterSelected += (characterData) => 
{
    Debug.Log($"Selecionado: {characterData.CharacterName}");
};
```
Obtendo o Personagem Selecionado

Para saber qual personagem está selecionado:

```csharp
SimpleStoreManager storeManager = FindObjectOfType<SimpleStoreManager>();
string selectedID = storeManager.GetSelectedCharacterID();

// Use o ID para ativar/desativar GameObjects, trocar materiais, etc.
if (selectedID == "character_1")
{
    // Lógica para personagem 1
}
```

## 📊 Sistema de Eventos

Você pode escutar mudanças de moeda:

```csharp
private void OnEnable()
{
    CurrencyManager.Instance.OnCurrencyChanged += OnCurrencyChanged;
}

private void OnDisable()
{
    ifAdicionar `SimpleStoreManager` ao painel da loja
- [ ] Configurar Currency Text no SimpleStoreManager
- [ ] Adicionar `SimpleStoreCharacter` a cada personagem na UI
- [ ] Definir Character ID único para cada um
- [ ] Definir preços
- [ ] Marcar um personagem como `Is Default = true`
- [ ] Configurar botões e referências UI
- [ ] Testar compra e seleção
- [ ] Verificar persistência (fechar e reabrir o jogo)

## 🐛 Troubleshooting

**Moedas não estão sendo salvas:**
- Verifique se completou uma fase (LevelCompleted adiciona moedas)
- Confirme que o CurrencyManager foi criado automaticamente

**Botões não funcionam:**
- Verifique se os botões têm o componente Button
- Confirme que as referências foram arrastadas no Inspector

**Não consigo comprar personagens:**
- Verifique se tem moedas suficientes (complete uma fase)
- Confirme que o Character ID é único para cada personagem

**Personagem não permanece selecionado:**
- Verifique se o Character ID está correto
- Confirme que apenas um personagem tem IsDefault marcado
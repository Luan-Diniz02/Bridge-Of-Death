# Sistema de Loja - Bridge Of Death

## 📋 Visão Geral

Sistema completo de loja para compra de personagens usando moedas coletadas durante o jogo. O sistema foi desenvolvido seguindo os princípios SOLID para garantir código limpo, manutenível e extensível.

## 🎯 Arquivos Criados

### 1. **CharacterData.cs** (ScriptableObject)
Define os dados de um personagem comprável:
- Nome, ID único, descrição
- Ícone e prefab do personagem
- Preço e status de personagem padrão

### 2. **StoreItem.cs**
Representa um item na loja com lógica de compra e seleção.

### 3. **CurrencyManager.cs** (Singleton)
Gerencia as moedas do jogador de forma persistente:
- Adicionar/gastar moedas
- Salvar/carregar usando PlayerPrefs
- Sistema de eventos para atualização de UI

### 4. **StoreManager.cs**
Gerencia toda a lógica da loja:
- Compra de personagens
- Seleção de personagem ativo
- Persistência de compras
- Integração com CurrencyManager

### 5. **StoreItemUI.cs**
Controla a UI de cada item na loja:
- Atualização visual baseada no estado
- Botões de compra e seleção
- Indicadores visuais

## 🚀 Como Usar

### Passo 1: Criar CharacterData

1. No Unity, clique com botão direito na pasta `Assets`
2. Vá em `Create > Store > Character Data`
3. Configure os dados do personagem:
   - **Character Name**: Nome exibido
   - **Character ID**: ID único (ex: "character_1")
   - **Description**: Descrição do personagem
   - **Character Icon**: Sprite do ícone
   - **Character Prefab**: Prefab do modelo 3D
   - **Price**: Preço em moedas
   - **Is Default**: Marque se for o personagem inicial gratuito

### Passo 2: Configurar a UI da Loja

1. **Criar o painel da loja** no Canvas do menu principal
2. **Adicionar componente StoreManager** ao GameObject da loja

3. **Estrutura recomendada da UI:**
```
Store Panel
├── Header
│   ├── Title Text ("LOJA")
│   └── Currency Display
│       ├── Coin Icon
│       └── Currency Text
└── Store Items Container (Grid Layout Group)
    └── [Items serão instanciados aqui]
```

4. **Criar Prefab do StoreItem:**
```
StoreItem Prefab
├── Background Image
├── Character Icon (Image)
├── Character Name (TextMeshProUGUI)
├── Price Container
│   ├── Coin Icon
│   └── Price Text
├── Purchase Button
│   └── Button Text
├── Select Button
│   └── Button Text
├── Locked Overlay (opcional)
└── Selected Indicator (opcional)
```

### Passo 3: Configurar o StoreManager

No Inspector do StoreManager, configure:
- **Currency Text**: Referência ao texto que mostra as moedas
- **Store Items Container**: Transform onde os itens serão instanciados
- **Store Item Prefab**: Prefab do UI do item
- **Available Characters**: Lista de CharacterData que estarão na loja

### Passo 4: Configurar o StoreItemUI Prefab

No prefab do item da loja, configure todas as referências:
- Character Icon, Name Text, Price Text
- Purchase Button e Select Button
- Locked Overlay (opcional)
- Selected Indicator (opcional)

### Passo 5: Integração com MenuManager

Adicione ao MenuManager.cs se necessário:

```csharp
[SerializeField] private StoreManager storeManager;

public void OpenStore()
{
    if(storeManager != null) 
    {
        storeManager.gameObject.SetActive(true);
    }
}
```

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

## 💾 Sistema de Persistência

Todos os dados são salvos automaticamente usando PlayerPrefs:
- **Moedas totais**: Salvas ao adicionar/gastar
- **Personagens comprados**: Salvos ao comprar
- **Personagem selecionado**: Salvo ao selecionar

Para resetar tudo:
```csharp
PlayerPrefs.DeleteAll();
```

## 🎨 Customização

### Cores e Estilos
Customize a aparência editando o prefab do StoreItem.

### Adicionar Novos Personagens
1. Crie um novo CharacterData
2. Adicione-o à lista `Available Characters` no StoreManager

### Modificar Preços
Edite o valor `Price` no CharacterData.

## ✅ Checklist de Configuração

- [ ] Criar pelo menos um CharacterData com `Is Default = true`
- [ ] Criar prefab do StoreItem com StoreItemUI component
- [ ] Configurar UI da loja no Canvas
- [ ] Adicionar StoreManager e configurar referências
- [ ] Adicionar CharacterData à lista do StoreManager
- [ ] Testar compra e seleção
- [ ] Verificar persistência (fechar e reabrir o jogo)

## 🐛 Troubleshooting

**Moedas não estão sendo salvas:**
- Verifique se o CurrencyManager está sendo criado (ele é um Singleton)
- Confirme que PlayerPrefs.Save() está sendo chamado

**Personagens não aparecem na loja:**
- Verifique se os CharacterData estão na lista `Available Characters`
- Confirme que o prefab do StoreItem tem o componente StoreItemUI

**Não consigo comprar personagens:**
- Verifique se tem moedas suficientes
- Confirme que o personagem não está marcado como já comprado

## 🎯 Próximos Passos Sugeridos

1. **Animações**: Adicionar animações de compra e seleção
2. **Sons**: Adicionar feedback sonoro para compras
3. **Preview 3D**: Mostrar modelo 3D do personagem na loja
4. **Categorias**: Organizar personagens por categorias
5. **Ofertas**: Sistema de ofertas/descontos temporários
6. **Conquistas**: Desbloquear personagens por conquistas além de compra

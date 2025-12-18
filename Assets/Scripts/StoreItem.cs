using UnityEngine;
using System;

/// <summary>
/// Representa um item na loja (Single Responsibility)
/// </summary>
[Serializable]
public class StoreItem
{
    [SerializeField] private CharacterData characterData;
    
    private bool isPurchased;
    private bool isSelected;
    
    public CharacterData CharacterData => characterData;
    public bool IsPurchased
    {
        get => isPurchased;
        set => isPurchased = value;
    }
    
    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }
    
    public StoreItem(CharacterData data)
    {
        characterData = data;
        isPurchased = data.IsDefault; // Personagens padrão já vêm desbloqueados
        isSelected = false;
    }
    
    public bool CanPurchase(int currentCurrency)
    {
        return !isPurchased && currentCurrency >= characterData.Price;
    }
    
    public bool Purchase()
    {
        if (isPurchased)
            return false;
            
        isPurchased = true;
        return true;
    }
}

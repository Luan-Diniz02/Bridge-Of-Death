# 🌉 Bridge of Death: Sobreviva ou Morra Tentando

[![Unity](https://img.shields.io/badge/Unity-6000.3.1f1-lightgrey.svg)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-Programming-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Plataforma](https://img.shields.io/badge/Plataforma-Windows%20%7C%20Linux-green.svg)]()

> **[Clique aqui para ver a apresentação visual do projeto (Gamma)](https://gamma.app/docs/Bridge-of-Death-Sobreviva-ou-Morra-Tentando-8j71j6iu8nrmypj)**

## 📖 Sobre o Projeto
**Bridge of Death** é um jogo 3D de sobrevivência e percurso para desktop (Windows e Linux). O jogador é desafiado a atravessar a tensa Ponte São Félix até a Nova Marabá, tendo de sobreviver a um surto viral no ano de 2026 ao longo de um trajeto de 2.1km. O foco central é a gestão de recursos, desvio de obstáculos e a sobrevivência pura.

## 🎮 Jogabilidade e Mecânicas
O objetivo principal é alcançar o final do nível gerenciando itens e reflexos.
* **Inimigos e Obstáculos:** Zumbis com movimentação randômica que causam dano por contato e barricadas estáticas que exigem desvio rápido.
* **Sistema de Power-ups:**
  * **Moedas:** Acumuláveis para desbloquear novos personagens na Loja.
  * **Coração:** Restaura pontos de vida perdidos.
  * **Escudo de Invencibilidade:** Permite atropelar zumbis sem sofrer dano temporariamente.
  * **Speed-up:** Aumento temporário da velocidade de corrida.
* **Personagens Jogáveis:** A aventura conta com três sobreviventes: Luan, Frank e Saulo.

## 🛠️ Arquitetura Técnica e Tecnologias
Este projeto foi desenvolvido do zero na **Unity (versão 6000.3.1f1)** utilizando a linguagem **C#**, aplicando conceitos de engenharia de software para garantir um código limpo e otimizado.

### 🧠 Integração com IA e Ferramentas Externas
* **Modelagem 3D Original:** Para garantir a originalidade visual sem recorrer a *assets* prontos da loja, todos os personagens foram gerados com recurso à Inteligência Artificial através do **Meshy AI** e **Tripo 3D**.
* **Animações Dinâmicas:** Utilização do **Mixamo** para implementar 6 animações base (Idle, Run, Jump, Slide, Damage, Death), controladas via *Animator Controller* com parâmetros de transição (float/trigger).
* **Materiais e Cenários:** Integração de texturas realistas para asfalto, metal e água provenientes da Unity Asset Store, configurando os *Materials* dos *Game Objects*.

### ⚙️ Lógica e Scripts Principais
A base de código foi modularizada através do padrão de *Managers* para gerenciar as regras globais do jogo:
* **PlayerController.cs & EnemyController.cs:** Scripts responsáveis pelas mecânicas de movimento do jogador e pela IA básica de patrulha/movimentação dos zumbis.
* **SpawnManager.cs & LaneController.cs:** Controle da geração procedural de elementos no cenário e gestão das faixas de movimento.
* **LevelCompleted.cs:** Gestão do estado do jogo, controlando as condições de "You Win" e "Game Over" de forma global.
* **CurrencyManager.cs & CoinManager.cs:** Implementação da economia interna do jogo, gerenciando a coleta de moedas e o desbloqueio de conteúdo na Loja.
* **Interação Otimizada:** Uso de gatilhos invisíveis (`OnTriggerEnter` com *Tags* "Coletavel") para processar a coleta de itens de forma fluida, evitando o custo computacional de colisões físicas duras.
* **Interface de Usuário (UI):** Criação de menus, HUDs (contador de vida e moedas) e botões responsivos utilizando o componente avançado **TextMeshPro**, conectando a interface gráfica aos scripts via métodos `OnClick()`.
* **Áudio:** Implementação de `AudioSource` para separar a música de tensão do *gameplay*, a música do Menu Principal e os efeitos sonoros de impacto e coleta de itens.

## 🚀 Como Jogar
1. Acesse a aba **Releases** na lateral direita deste repositório.
2. Faça o *download* do arquivo `.zip` com a versão jogável (v1.0).
3. Extraia o conteúdo para uma pasta no seu computador.
4. Execute o arquivo `BridgeOfDeath.exe` e divirta-se!

## 👥 Equipe de Desenvolvimento
Este projeto foi desenvolvido como aplicação prática da disciplina de Projetos em Engenharia na UNIFESSPA.
* **Desenvolvedores:** Frank Fábio Santos da Silva, Luan Pereira Diniz e Saulo Gomes Martins.
* **Orientador:** Prof. Manoel Ribeiro Filho.

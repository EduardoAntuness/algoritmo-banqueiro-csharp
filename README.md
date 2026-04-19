# 🏦 Algoritmo do Banqueiro (C#)

## 📌 Visão Geral

Este projeto implementa o **Algoritmo do Banqueiro**, uma técnica clássica de sistemas operacionais utilizada para **prevenir deadlocks** em ambientes com múltiplos processos concorrentes.

A aplicação simula um sistema onde vários clientes (threads) solicitam e liberam recursos, e o algoritmo garante que **somente estados seguros sejam permitidos**, evitando travamentos do sistema.

---

## 🎯 Objetivos do Projeto

* Implementar o algoritmo do banqueiro na prática
* Simular concorrência com múltiplas threads
* Garantir segurança na alocação de recursos
* Evitar deadlocks e condições de corrida

---

## 🧠 Conceitos Aplicados

* Concorrência e paralelismo
* Threads e sincronização
* Deadlock e prevenção
* Algoritmo de segurança
* Controle de recursos compartilhados

---

## ⚙️ Tecnologias Utilizadas

* C#
* .NET (Console Application)
* System.Threading

---

## 🏗️ Estrutura do Projeto

algoritmo-banqueiro-csharp/
├── Program.cs
├── README.md

---

## ▶️ Como Executar

### 🔹 Pré-requisitos

* Ter o .NET SDK instalado

### 🔹 Execução

No terminal, dentro da pasta do projeto:

dotnet run 10 5 7

📌 Onde:

* 10 = quantidade do recurso 1
* 5 = quantidade do recurso 2
* 7 = quantidade do recurso 3

---

## 🔄 Funcionamento do Sistema

1. O sistema inicia com uma quantidade fixa de recursos disponíveis
2. Cada cliente possui:

   * Demanda máxima
   * Recursos alocados
   * Necessidade restante
3. Threads simulam clientes solicitando recursos aleatórios
4. O sistema verifica:

   * Se há recursos disponíveis
   * Se o estado continua seguro após a alocação
5. Resultado:

   * ✅ Estado seguro → recursos são concedidos
   * ❌ Estado inseguro → solicitação é negada

---

## 🔒 Controle de Concorrência

Para evitar condições de corrida, foi utilizado mecanismo de exclusão mútua:

lock (lockObj)

Isso garante que apenas uma thread acesse os dados compartilhados por vez, mantendo a integridade do sistema.

---

## 📊 Exemplo de Saída

Cliente 1 solicitou recursos
Cliente 3 solicitou recursos
Cliente 0 solicitou recursos

---

## 📚 Referência Bibliográfica

SILBERSCHATZ, Abraham; GALVIN, Peter; GAGNE, Greg.
Fundamentos de Sistemas Operacionais. 9ª edição.

---

## 👨‍💻 Autor

Eduardo Antunes

---


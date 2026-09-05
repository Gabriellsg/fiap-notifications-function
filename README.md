# FIAP Notifications Function (Serverless)

Azure Functions (.NET 8 Isolated) no lugar da `fiap-notifications-api` (container).

## O que faz

| Function | Fila RabbitMQ | Origem |
|----------|---------------|--------|
| `UsuarioCriadoFunction` | `usuario-criado` | UsersAPI (cadastro) |
| `PagamentoProcessadoFunction` | `pagamento-processado-notifications` | PaymentsAPI (fanout `pagamento-processado`) |

O e-mail continua **simulado** via `ILogger` (mesmo comportamento da API antiga).

## Pré-requisitos

- .NET 8 SDK
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) v4
- RabbitMQ do `fiap-orchestration` (`docker compose up -d rabbitmq`)

## Rodar local

```bash
# 1) Subir o broker (pasta fiap-orchestration)
docker compose up -d rabbitmq

# 2) Nesta pasta
cd src/FiapCloudGames.Notifications.Functions
func start
```

`local.settings.json` já aponta para `amqp://admin:rabbitmq123@localhost:5672/`.

**Importante:** pare o container `notifications-api` no compose para não haver dois consumers na mesma fila.

## Testar

1. Cadastre um usuário (via Kong ou Users API) → log `Enviando e-mail para ... Bem-vindo`.
2. Processe um pagamento → log de pedido aprovado/recusado.

## Deploy Azure (IaC)

```bash
az group create -n fiap-notifications-rg -l brazilsouth
az deployment group create \
  -g fiap-notifications-rg \
  -f infra/main.bicep \
  --parameters rabbitMqConnectionString='amqp://user:pass@host:5672/'
```

Depois publique o projeto Functions no Function App criado.

## Estrutura

```
src/
  FiapCloudGames.Notifications.Application/   # Events + serviços
  FiapCloudGames.Notifications.Infrastructure/# Simulador e-mail + topologia RabbitMQ
  FiapCloudGames.Notifications.Functions/     # Triggers
infra/
  main.bicep
```

## Correção de contrato

A UsersAPI publica `Id`; a API antiga esperava `UsuarioId`. Esta Function aceita os dois (`UsuarioResolvido`).

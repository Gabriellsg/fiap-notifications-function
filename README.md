# FIAP Notifications Function (Serverless)

Azure Functions (.NET 8 Isolated) no lugar da `fiap-notifications-api` (container).

## O que faz

| Function | Fila RabbitMQ | Origem |
|----------|---------------|--------|
| `UsuarioCriadoFunction` | `usuario-criado` | UsersAPI (cadastro) |
| `PagamentoProcessadoFunction` | `pagamento-processado-notifications` | PaymentsAPI (fanout `pagamento-processado`) |

O e-mail continua **simulado** via `ILogger` (mesmo comportamento da API antiga).

UsersAPI e PaymentsAPI **não mudam**: continuam publicando nas mesmas filas/exchanges.

## Pré-requisitos

- .NET 8 SDK
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) v4 (`func --version`)
- Stack do `fiap-orchestration` (RabbitMQ + Users + Catalog + Payments)
- PowerShell (comandos abaixo usam `Invoke-RestMethod`)

## 1) Subir a stack

Na pasta `fiap-orchestration`:

```powershell
cd C:\Workspace\FIAP\fiap-orchestration
docker compose up -d --remove-orphans
docker ps
```

Confirme healthy: `rabbitmq`, `users-api` (8080), `catalog-api` (8082), `payments-api` (8083).

O serviço `notifications-api` **não** sobe no compose (evita dois consumers na mesma fila). Se ainda existir container antigo:

```powershell
docker stop notifications-api
docker rm notifications-api
```

## 2) Configurar e subir a Function

```powershell
cd C:\Workspace\FIAP\fiap-notifications-function\src\FiapCloudGames.Notifications.Functions
Copy-Item local.settings.json.example local.settings.json -Force
func start
```

Deixe esse terminal aberto. Deve listar:

- `UsuarioCriadoFunction` (`rabbitMQTrigger`)
- `PagamentoProcessadoFunction` (`rabbitMQTrigger`)

> **Aviso amarelo `AzureWebJobsStorage` / Unhealthy:** comum sem Azurite. Não impede os triggers RabbitMQ — pode ignorar neste teste.
>
> Se o PowerShell não achar `func` após instalar as Core Tools, feche e abra o terminal de novo, ou rode:
> ```powershell
> $env:Path = [System.Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path','User')
> ```

Os testes abaixo vão em **outro** PowerShell.

## 3) Testar usuário criado → `UsuarioCriadoFunction`

```powershell
$email = "gabriel$(Get-Random)@fiap.com"
$usuario = Invoke-RestMethod -Method Post -Uri http://localhost:8080/api/Usuarios `
  -ContentType 'application/json' `
  -Body (@{ nome='Gabriel'; email=$email; senha='Senha@123' } | ConvertTo-Json)
$usuarioId = $usuario.id
Write-Output "usuarioId=$usuarioId email=$email"
```

**Esperado no `func start`:**

- `RabbitMQ message detected from queue: usuario-criado`
- `Enviando e-mail para ... Bem-vindo, Gabriel! Seu usuário foi criado com sucesso.`

## 4) Testar pagamento → `PagamentoProcessadoFunction`

Usa o `$usuarioId` e `$email` do passo 3.

### 4.1 Criar jogo

```powershell
$jogo = Invoke-RestMethod -Method Post -Uri http://localhost:8082/api/Jogos `
  -ContentType 'application/json' `
  -Body (@{ Titulo='FIFA 26'; Descricao='Futebol'; Preco=199.90 } | ConvertTo-Json)
$jogoId = $jogo.id
Write-Output "jogoId=$jogoId"
```

### 4.2 Criar biblioteca

```powershell
$bib = Invoke-RestMethod -Method Post -Uri http://localhost:8082/api/Bibliotecas `
  -ContentType 'application/json' `
  -Body (@{ UsuarioId=$usuarioId } | ConvertTo-Json)
$bibId = $bib.id
Write-Output "bibliotecaId=$bibId"
```

### 4.3 Solicitar adição do jogo (dispara o fluxo)

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:8082/api/Bibliotecas/$bibId/jogos" `
  -ContentType 'application/json' `
  -Body (@{ JogoId=$jogoId; NomeUsuario='Gabriel'; Email=$email } | ConvertTo-Json)
```

Fluxo: Catalog → fila `pedido-criado` → Payments → fanout `pagamento-processado` → Function.

**Esperado no `func start`:**

- `RabbitMQ message detected from queue: pagamento-processado-notifications`
- `Enviando e-mail ... pedido ... processado com sucesso` **ou** `não foi aprovado`

O status (Aprovado/Recusado) é **aleatório** no Payments — os dois textos são válidos.

## 5) Troubleshooting

| Sintoma | O que checar |
|---------|----------------|
| `func` não reconhecido | Reabrir o terminal / atualizar PATH (passo 2) |
| Cadastro ok, sem log na Function | `func start` aberto? RabbitMQ healthy? `notifications-api` parado? |
| Sem log de pagamento | `payments-api` e `catalog-api` healthy? `$usuarioId` / `$jogoId` / `$bibId` impressos? |
| Kong `404 no Route matched` | Use as portas diretas `8080` / `8082` nos testes acima (não depende do Gateway) |
| PowerShell + `curl.exe` com JSON quebrado | Prefira `Invoke-RestMethod` + `ConvertTo-Json` como nos exemplos |

## Deploy Azure (IaC)

```powershell
az group create -n fiap-notifications-rg -l brazilsouth
az deployment group create `
  -g fiap-notifications-rg `
  -f infra/main.bicep `
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

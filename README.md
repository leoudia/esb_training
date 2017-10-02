ESB Training
===================


Barramento de serviço **REST** desenvolvido com o propósito de aprendizado do conceito de barramento.

Criado na plataforma **.Net Core**, o barramento explora o uso de Middleware do projeto Web API.

Publicando as APIs
-------
Para registrar uma API no barramento, basta realizar uma chamada POST conforme o exemplo abaixo:


curl -v -H "Accept: application/json" -H "Content-type: application/json" -X POST -d '{"ApiName": "resttest", "Version": "v1", "Method": "POST", "Endpoint": "https://httpbin.org/post"}'  http://localhost:5000/api/Endpoint

**Body - JSON**

{
  "ApiName": "Contexto da API",
  "Version": "Versão",
  "Method": "POST, GET, PUT, DELETE",
  "Endpoint": "URL"
}

Consumindo as APIs
-------
Conforme o exemplo acima, publicamos uma api e iremos consumir a mesma a partir do barramento de serviço, exemplo:

curl -v -H "Accept: application/json" -H "Content-type: application/json" -X POST -d '{"esb": "first api", "id": 1, "public": true}'  http://localhost:5000/resttest/v1/post?p=1&m=2

localhost:5000/**resttest**/v1/post?p=1&m=2

Contexto da API cadastrada, a partir desse contexto é gerado uma rota para cada API do barramento.

localhost:5000/resttest/**v1**/post?p=1&m=2

Versão da API, com esse parâmetro permite uma melhor gestão das APIs no barramento, ou seja, podemos ter uma versão da sua API para uma versão específica do seu aplicativo mobile sem quebrar uma versão anterior devido a alguma feature que alterou o comportamento da API.

localhost:5000/resttest/v1/**post**?p=1&m=2

Recurso desejado na API de destino.

Manipulando Requisições com MiddleHandle
-------

É possível interceptar requisições HTTP ou até negá-las com MiddleHandle.

**IMiddleHandle**

bool CanHandle(ESBContext context, out string msg, out int code);

Através desse metódo é possível implementar uma regra de negócio que decidira se a requisição pode ser aceita.
Dessa forma é possível criar um estrutura de segurança para aceitar requisições de usuários que tenha permissão para
acessar o recurso específicado na requisição.

void RequestInterceptor(ESBContext context);

Esse metodo é invocado antes que a requisção seja despachada. Pode ser utilizado para adicionar alguma informação no header
da requisição, modificar o payload da requisição e etc.

void ResponseInterceptor(ESBContext context);

Quando a resposta chega da API que contém o recurso desejado, é chamado esse metódo antes de responder para o
para a aplição que solicitou o barramento.

**Registrando o IMiddleHandle**

Para registrar o handle customizado, basta utilizar o factory "ESBFactory" e chamar o metódo "RegisterMiddle" passando o handle como parâmetro.

ESBFactory.Instance.RegisterMiddle(...)

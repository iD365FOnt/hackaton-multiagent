using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents;



class Program
{
    static async Task Main(string[] args)
    {
        // TODO: Reemplaza con tu endpoint y clave de Azure OpenAI
        string azureOpenAIEndpoint = "https://xxxxxxxx.openai.azure.com/";
        string azureOpenAIKey = "xxxxxxxxxxxxxxxxxxxxxxxxxx";

        // TODO: Reemplaza con tu cadena de conexión de Azure AI Foundry
        string connectionString = "xxxxxxx.api.azureml.ms;xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx;xxxxxxxxxxxxxxx;xxxxxxxxxxxxxxxxxx";

        // Inicializa el kernel
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion("gpt-4o", azureOpenAIEndpoint, azureOpenAIKey);
        Kernel kernel = builder.Build();

        // Inicializa el cliente de agentes de Azure
        var credential = new DefaultAzureCredential();
        var client = new AgentsClient(connectionString, credential);

        // Diccionario de agentes con sus IDs
        var agentIds = new Dictionary<string, string>
        {
            { "orchestratorAgent", "asst_xxxxxxxxxxxxxxxxxxxxx"},
            { "sharepointAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "Dynamics365Query", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "crmAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "dataverseAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "clickUpAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "managementAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "hrAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "supportAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "developerAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "consultantAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "projectManagerAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "marketingAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "azureAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "openHrAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
            { "devOpsAgent", "asst_xxxxxxxxxxxxxxxxxxxxx" },
        };

        #pragma warning disable SKEXP0110 // Type or member is obsolete
                var agents = new List<AzureAIAgent>();
        #pragma warning restore SKEXP0110 // Type or member is obsolete

        // Recupera y configura cada agente
        foreach (var kvp in agentIds)
        {
            var agentResponse = await client.GetAgentAsync(kvp.Value);
            var agentDefinition = agentResponse.Value;
            #pragma warning disable SKEXP0110 // Suppress warning for AzureAIAgent being for evaluation purposes only
                        var agent = new AzureAIAgent(agentDefinition, client)
                        {
                            Kernel = kernel
                        };
            #pragma warning restore SKEXP0110 // Re-enable warning
                        agents.Add(agent);
        }


        #pragma warning disable SKEXP0110
        // Definir la función de selección
        var selectionFunction = AgentGroupChat.CreatePromptFunctionForStrategy(
            """
            Basado en el último mensaje, determina qué agente debe responder a continuación.
            Solo responde con el nombre del agente sin explicación adicional.
            Agentes disponibles:
            - orchestratorAgent
            - sharepointAgent
            - Dynamics365Query
            - crmAgent
            - dataverseAgent
            - clickUpAgent
            - managementAgent
            - hrAgent
            - supportAgent
            - developerAgent
            - consultantAgent
            - projectManagerAgent
            - marketingAgent
            - azureAgent
            - openHrAgent
            - devOpsAgent

            Último mensaje:
            {{$lastmessage}}
            """,
            safeParameterNames: "lastmessage"
        );

        // Definir la función de terminación
        var terminationFunction = AgentGroupChat.CreatePromptFunctionForStrategy(
            """
            Analiza el último mensaje y determina si la conversación debe finalizar.
            Si el mensaje indica que no hay más acciones necesarias, responde con "yes".
            De lo contrario, responde con "no".
            Último mensaje:
            {{$lastmessage}}
            """,
            safeParameterNames: "lastmessage"
        );

        var orchestratorAgent = agents.FirstOrDefault(a => a.Name == "orchestratorAgent");
        if (orchestratorAgent == null)
        {
            throw new InvalidOperationException("El agente orquestador no se encontró en la lista de agentes.");
        }


        // Configura el sistema multiagente
        var chat = new AgentGroupChat(agents.ToArray())
        {
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                {
                    InitialAgent = orchestratorAgent,
                    HistoryVariableName = "lastmessage",
                    ResultParser = result => result?.GetValue<string>() ?? orchestratorAgent?.Name ?? string.Empty,
                    HistoryReducer = new ChatHistoryTruncationReducer(1)
                },
                TerminationStrategy = new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                {
                    ResultParser = result =>
                    {
                        var val = result.GetValue<string>() ?? string.Empty;
                        return val.Contains("yes", StringComparison.OrdinalIgnoreCase);
                    },
                    HistoryVariableName = "lastmessage",
                    HistoryReducer = new ChatHistoryTruncationReducer(10),
                    MaximumIterations = 10,
                }
            }
        };

        #pragma warning restore SKEXP0110 // Suppress warning for AgentGroupChat being for evaluation purposes only


        while (true)
        {
            Console.Write("Tú: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("salir", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Saliendo del chat...");
                break;
            }

            // Agrega el mensaje del usuario al chat
            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));

            // Procesa la conversación
            await foreach (var content in chat.InvokeAsync())
            {
                #pragma warning disable SKEXP0001 // Suppress warning for AuthorName being for evaluation purposes only
                Console.WriteLine($"{content.Role} - {content.AuthorName}: {content.Content}");
                #pragma warning restore SKEXP0001 // Re-enable warning
            }
        }


    }
}
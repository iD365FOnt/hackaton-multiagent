# MultiAgentApp - Orquestador de Agentes Inteligentes 

[![Nombre del Hackathon](https://img.shields.io/badge/Hackathon-AI%20Agents%20Hackathon%202025-blue.svg)](https://enlace.del.hackathon.com) [![Licencia](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Tabla de Contenidos

* [Descripción del Proyecto](#descripción-del-proyecto)
* [Arquitectura y Componentes Principales](#arquitectura-y-componentes-principales)
* [Tecnologías Utilizadas](#tecnologías-utilizadas)
* [Instalación y Ejecución](#instalación-y-ejecución)
* [Uso Básico](#uso-básico)
* [Licencia](#licencia)
* [Diagrama de Dependencias de Agentes](#diagrama-de-dependencias-de-agentes)
## Descripción del Proyecto

MultiAgentApp es una solución innovadora que orquesta una variedad de agentes inteligentes para realizar diversas tareas y flujos de trabajo. La arquitectura se centra en un orquestador central que coordina agentes interactivos y no interactivos, permitiendo una colaboración eficiente y la automatización de procesos complejos.

Este proyecto ha sido desarrollado con el objetivo de [Describe brevemente el objetivo principal del proyecto para el hackathon. Por ejemplo: "demostrar una arquitectura escalable para la integración de múltiples servicios a través de agentes inteligentes", "automatizar un flujo de trabajo específico utilizando la colaboración entre diferentes agentes", etc.].

## Arquitectura y Componentes Principales

La arquitectura de MultiAgentApp se basa en los siguientes componentes principales:

* **Orchestrator (Orquestador):** El componente central que gestiona y dirige la interacción entre los diferentes agentes. Actúa como el cerebro de la aplicación, decidiendo qué agente debe activarse y cómo se deben compartir los datos entre ellos. Es el punto de entrada y coordinador principal del sistema que delega tareas a los agentes interactivos.
* **Agentes Interactivos:** Agentes diseñados para interactuar directamente con el orquestador y, en algunos casos, entre sí para llevar a cabo tareas de alto nivel. Estos incluyen:
    * Management Agent
    * HR Agent
    * Support Agent
    * Developer Agent
    * Consultant Agent
    * Project Manager Agent
    * Marketing Agent
* **Agentes No Interactivos:** Agentes especializados que proporcionan acceso a servicios o datos específicos. Son utilizados por los agentes interactivos para realizar tareas concretas. Encapsulan la lógica de comunicación con servicios externos específicos (Azure, Fabric, OpenHR, Dataverse, Dynamics 365, ClickUp, CRM, SharePoint) utilizando las definiciones OpenAPI (`.yaml`) proporcionadas. Estos incluyen:
    * Fabric Agent
    * Azure Agent
    * OpenHR Agent
    * Dataverse Agent
    * Dynamics365 Query Agent
    * ClickUp Agent
    * CRM Agent
    * SharePoint Agent
* **APIs Externas (OpenAPI):** Se utilizan archivos de definición OpenAPI (`.yaml`) para describir las interfaces de los servicios externos con los que interactúan los agentes no interactivos.
## Tecnologías Utilizadas

* **.NET / C#:** Lenguaje y framework principal de desarrollo.
* **Visual Studio:** Entorno de desarrollo integrado (IDE).
* **OpenAPI (Swagger):** Para la definición y documentación de las APIs externas consumidas.
* **Mermaid:** Para la generación del diagrama de arquitectura directamente en Markdown.
* **Azure AI Studio:** Plataforma unificada para el desarrollo, entrenamiento, despliegue y orquestación de soluciones de Inteligencia Artificial, incluyendo agentes.
* **Azure API Management:** Servicio para publicar, asegurar, transformar, mantener y monitorizar APIs (útil para exponer o consumir APIs de los agentes/orquestador).

## Instalación y Ejecución

1.  **Clonar el repositorio:**
    ```bash
    git clone iD365FOnt/hackaton-multiagent
    cd hackaton-multiagent/MultiAgentApp
    ```
2.  **Abrir en Visual Studio:** Abre el archivo de solución `MultiAgentApp.sln`.
3.  **Restaurar Dependencias:** Visual Studio debería restaurar automáticamente los paquetes NuGet necesarios. Si no, hazlo manualmente (Click derecho en la solución -> Restaurar paquetes NuGet).
4.  **Configuración:** Es posible que necesites configurar credenciales, URLs de API u otros parámetros para los Agentes No Interactivos (revisa el código fuente o añade un archivo de configuración si es necesario).
5.  **Compilar y Ejecutar:** Compila la solución (Build Solution) y ejecuta el proyecto `MultiAgentApp` (probablemente como aplicación de consola o servicio, revisa `Program.cs`).

## Uso Básico

Una vez en ejecución, el Orquestador está listo para recibir instrucciones (el método exacto dependerá de cómo esté implementado: API REST, consola, cola de mensajes, etc.).

*Ejemplo de flujo (conceptual):*

1.  Una petición llega al Orquestador (ej: "Crear tarea en ClickUp para revisar bug #123").
2.  El Orquestador identifica que esta tarea corresponde al dominio del `Project Manager Agent`.
3.  El Orquestador delega la petición al `Project Manager Agent`.
4.  El `Project Manager Agent` necesita interactuar con ClickUp, así que utiliza el `ClickUp Agent` (no interactivo).
5.  El `ClickUp Agent` realiza la llamada a la API de ClickUp (usando la definición `ClickUp.openapi.yaml`).
6.  El resultado (éxito/error) se propaga de vuelta hasta el origen si es necesario.

## Licencia

Este proyecto se distribuye bajo la licencia [Elige una Licencia, ej: MIT]. Consulta el archivo `LICENSE` para más detalles.

## Diagrama de Dependencias de Agentes

```mermaid
graph TD
    %% Orquestador
    Orchestrator[Orchestrator]

    %% Agentes Interactivos
    managementAgent[Management Agent]
    hrAgent[HR Agent]
    supportAgent[Support Agent]
    developerAgent[Developer Agent]
    consultantAgent[Consultant Agent]
    projectManagerAgent[Project Manager Agent]
    marketingAgent[Marketing Agent]

    %% Relaciones Orquestador-Interactivos
    Orchestrator --> managementAgent
    Orchestrator --> hrAgent
    Orchestrator --> supportAgent
    Orchestrator --> projectManagerAgent
    Orchestrator --> marketingAgent

    %% Relaciones internas Interactivos
    managementAgent --> hrAgent
    managementAgent --> marketingAgent
    projectManagerAgent --> consultantAgent
    projectManagerAgent --> developerAgent
    projectManagerAgent --> supportAgent
    supportAgent --> developerAgent

    %% Agentes No Interactivos (Repetidos según conexión)
    %% Management Agent - No interactivos
    managementAgent --> fabricAgent1[Fabric Agent]
    managementAgent --> azureAgent1[Azure Agent]

    %% HR Agent - No interactivos
    hrAgent --> openHrAgent1[OpenHR Agent]
    hrAgent --> dataverseAgent1[Dataverse Agent]

    %% Support Agent - No interactivos
    supportAgent --> Dynamics365Query1[Dynamics365 Query Agent]
    supportAgent --> clickUpAgent1[ClickUp Agent]

    %% Developer Agent - No interactivos
    developerAgent --> Dynamics365Query2[Dynamics365 Query Agent]
    developerAgent --> azureAgent2[Azure Agent]
    developerAgent --> clickUpAgent2[ClickUp Agent]

    %% Consultant Agent - No interactivos
    consultantAgent --> Dynamics365Query3[Dynamics365 Query Agent]
    consultantAgent --> clickUpAgent3[ClickUp Agent]
    consultantAgent --> crmAgent1[CRM Agent]

    %% Project Manager Agent - No interactivos
    projectManagerAgent --> clickUpAgent4[ClickUp Agent]
    projectManagerAgent --> sharepointAgent1[SharePoint Agent]
    projectManagerAgent --> WebAlgoritmia1[Web Algoritmia Agent]

    %% Marketing Agent - No interactivos
    marketingAgent --> crmAgent2[CRM Agent]
    marketingAgent --> WebAlgoritmia2[Web Algoritmia Agent]

    %% Estilos
    classDef interactivo fill:#0D47A1,color:#ffffff,stroke:#ffffff,stroke-width:1px
    classDef nointeractivo fill:#ffffff,color:#000000,stroke:#000000,stroke-width:1px
    classDef orchestrator fill:#1565C0,color:#ffffff,stroke:#ffffff,stroke-width:2px

    %% Aplicación de estilos
    class Orchestrator orchestrator
    class managementAgent,hrAgent,supportAgent,developerAgent,consultantAgent,projectManagerAgent,marketingAgent interactivo
    class fabricAgent1,azureAgent1,openHrAgent1,dataverseAgent1,Dynamics365Query1,clickUpAgent1,Dynamics365Query2,azureAgent2,clickUpAgent2,Dynamics365Query3,clickUpAgent3,crmAgent1,clickUpAgent4,sharepointAgent1,WebAlgoritmia1,crmAgent2,WebAlgoritmia2 nointeractivo

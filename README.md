<p align="center">
  <img src="/TTS_Live_UI/Assets/StoreLogo.scale-400.png"/>
</p>
<h3 align="center">Text To Speech Live UI by CrashXBETAX 0.1.0.0</h3>
<br>
<p align="center">
  <a href="https://github.com/CrashXBETAX/Text_To_Speech_Live_WinUI3_Public/issues"><strong>Report Bug »</strong></a>
</p>
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#sobre-o-projeto">Sobre o projeto</a>
    </li>
    <li><a href="#instalação">Instalação</a></li>
    <li><a href="#recursos">Recursos</a></li>
    <li><a href="#uso">Uso</a></li>
    <li><a href="#licença">Licença</a></li>
    <li><a href="#contato">Contato</a></li>
  </ol>
</details>

## Sobre o projeto
### Objetivo:

Text To Speech Live UI é um aplicativo que usa as tecnologias .NET 10 e WinUI 3 para converter texto em voz. O aplicativo permite que os usuários digitem ou colem um texto e escolham uma voz sintetizada para reproduzi-lo em áudio. O aplicativo oferece controles integrados para ajustar o volume, a velocidade e a seleção de voz. Além disso, o aplicativo pode ser usado para falar no microfone usando um adaptador ou driver de áudio, permitindo que os usuários se comuniquem com outras pessoas ou dispositivos por meio da voz. O aplicativo também permite que os usuários usem textos prontos (atalhos) para falar uma frase frequente, economizando tempo e esforço. Text To Speech UI é um projeto que visa facilitar a comunicação e o aprendizado para pessoas com dificuldades de leitura ou audição.

## Instalação

### Versão da Microsoft Store
<a href="https://apps.microsoft.com/store/detail/9MWHWTD64HPL?launch=true&mode=full">
	<img src="https://get.microsoft.com/images/pt-br%20dark.svg"  width="250"/>
</a>

### Versão Código-fonte do GitHub
- <a href="https://visualstudio.microsoft.com/pt-br/downloads/">Instale a IDE Visual Studio 2022</a>
- <a href="https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=cs-vs-community%2Ccpp-vs-community%2Cvs-2022-17-1-a%2Cvs-2022-17-1-b">Instale os componentes de WinUI 3</a>
- Clone o repositório Text_To_Speech_Live_WinUI3_Public
- Abra e compile a solução na Visual Studio 2022

## Recursos

### Text To Speech
Digite ou cole qualquer texto e converta em áudio com um clique. Use `Ctrl+Enter` para converter rapidamente.

### Modo Live
Com o recurso Live ativado, o programa começa a converter o texto em voz assim que o usuário digita, terminando a frase com `?`, `!`, `.`, `;`, `:` ou quebra de linha. O atraso (debounce) é configurável pelo usuário.

### Controles de Voz
- **Seleção de voz**: Escolha entre todas as vozes instaladas no sistema
- **Volume**: Ajuste de 0 a 100
- **Velocidade**: Ajuste de -10 a 10
- **Debounce do Live**: Ajuste o tempo de espera antes de falar no modo Live

### Atalhos de Texto
Crie frases prontas para falar rapidamente com um clique — útil para frases frequentes em reuniões e conversas online.

### Histórico de Conversões
Todas as conversões são registradas automaticamente. Acesse o histórico para reproduzir novamente ou remover entradas.

### Modo Clipboard Watch
Ative o monitoramento da área de transferência — qualquer texto copiado é automaticamente falado pelo TTS.

### Salvar Áudio
Salve o áudio gerado como arquivo **WAV** (`Ctrl+S`) ou **MP3** (`Ctrl+Shift+S`).

### Prioridade de Áudio
Ative a opção para reduzir automaticamente o volume de outros aplicativos enquanto o TTS está falando.

### Destaque Visual
A palavra sendo falada é destacada visualmente no editor de texto em tempo real.

### Hotkeys Globais
Hotkeys do sistema permitem controlar o TTS de qualquer lugar, sem precisar trazer a janela para frente.

### Localização
Interface disponível em Português (Brasil) e Inglês (EUA), com detecção automática do idioma do sistema.

## Uso
Para usar o aplicativo Text To Speech Live UI, digite no campo e clique em converter em áudio para iniciar. Ou ative o recurso Live antes de digitar. Use o botão de engrenagem para ajustar voz, volume e velocidade.

## Arquitetura
- **Framework**: .NET 10 + WinUI 3 (Windows App SDK 1.7)
- **Padrão**: MVVM com CommunityToolkit.Mvvm 8.4
- **TTS Engine**: System.Speech (SAPI) com SpeakProgress para highlighting
- **DI**: Microsoft.Extensions.Hosting
- **Logging**: Microsoft.Extensions.Logging
- **Persistência**: JSON (Newtonsoft.Json) para histórico e atalhos
- **Localização**: .resw (pt-BR, en-US)

## TO DO
- ✅ Melhorias na tela da configuração
- ✅ Permitir a ajuste do tamanho de fonte
- ✅ O recurso Live
- ✅ A tela da introdução
- ✅ Implementação de atalhos de texto
- ✅ Controles de volume, velocidade e voz
- ✅ Melhorias do recurso Live (debounce configurável, detecção melhorada)
- ✅ Implementação de Salvar Como
- ✅ Migração para .NET 10
- ✅ Arquitetura MVVM adequada com ISpeechService
- ✅ Logging e tratamento de exceções
- ✅ Bloquear sons de outros aplicativos e priorizar o TTS Live UI
- ✅ Exportação em formato MP3
- ✅ Histórico de conversões
- ✅ Modo Clipboard Watch
- ✅ Hotkeys globais do sistema
- ✅ Destaque visual da palavra sendo falada
- ✅ Localização multi-idioma completa (pt-BR e en-US)

## Licença
Esse projeto está sob licença MIT. Veja o arquivo [LICENÇA](LICENSE) para mais detalhes.<br>

## Contato
Gabriel Hagui

<a href="mailto:gabrielhagui@live.com" target="_blank"><img src="https://img.shields.io/badge/Microsoft_Outlook-0078D4?style=for-the-badge&logo=microsoft-outlook&logoColor=white" target="_blank"></a>

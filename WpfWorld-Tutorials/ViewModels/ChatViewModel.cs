using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WpfApp.Models;
using WpfApp.Services;


namespace WpfApp.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        CancellationTokenSource cancellationToken;
        public ObservableCollection<ChatMessage> Messages { get; } = new();
        OllamaService service = new();

        [ObservableProperty]
        private string userInput = string.Empty;
        [ObservableProperty]
        private bool isSendVisible = true;

        [RelayCommand]
        private async Task SendAsync()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
                return;
            IsSendVisible = false;
            Messages.Add(new ChatMessage
            {
                Content = UserInput,
                IsUser = true
            });

            var prompt = UserInput;
            UserInput = string.Empty;

            var aiMessage = new ChatMessage
            {
                Content = "Thinking...",
                IsUser = false
            };
            Messages.Add(aiMessage);
            bool firstChunk = true;
            cancellationToken = new CancellationTokenSource();
            try
            {
                await foreach (var chunk in service.StreamResponseAsync(prompt, cancellationToken.Token))
                {
                    if (firstChunk)
                    {
                        aiMessage.Content = chunk;
                        firstChunk = false;
                    }
                    else
                    {
                        aiMessage.Content += chunk;
                        await Task.Delay(2);
                    }
                }
                IsSendVisible = true;
            }
            catch (OperationCanceledException)
            {
                IsSendVisible=true;
                aiMessage.Content = "Stopped";
            }
        }

        [RelayCommand]
        private void Copy(string text)
        {
            Clipboard.SetText(text);
        }
        [RelayCommand]
        private void Stop()
        {
            cancellationToken.Cancel();
        }

    }

    /*
        public partial class ChatViewModel : ObservableObject
        {
            private readonly OllamaService _service;

            [ObservableProperty]
            private ObservableCollection<ChatMessage> messages = new();

            [ObservableProperty]
            private string userInput;

            public ChatViewModel(OllamaService service)
            {
                _service = service;

            }
    //private void DetectCode(ChatMessage msg)
        //{
        //    msg.IsCode = Regex.IsMatch(msg.Content, "```[\\s\\S]*?```");
        //}
            // 🔥 STREAM STATE (important)
            private bool _isInCodeBlock;
            private string _currentLanguage;
            private ChatSegment _currentSegment;
            private StringBuilder _tickBuffer = new(); // for detecting ```
            private StringBuilder _langBuffer = new();

            // =========================
            // SEND MESSAGE
            // =========================
            [RelayCommand]
            private async Task SendAsync()
            {


                if (string.IsNullOrWhiteSpace(UserInput))
                    return;

                // Add user message
                Messages.Add(new ChatMessage
                {
                    IsUser = true,
                    Segments = new ObservableCollection<ChatSegment>
                                {
                                    new ChatSegment
                                    {
                                        Text = UserInput,
                                        IsCode = false
                                    }
                                }
                });

                var prompt = UserInput;
                UserInput = string.Empty;

                // AI message
                var aiMessage = new ChatMessage { IsUser = false };
                Messages.Add(aiMessage);

                // Reset state
                ResetStreamState();

                // Optional: show thinking
                aiMessage.Segments.Add(new ChatSegment
                {
                    Text = "Thinking...",
                    IsCode = false
                });

                bool firstChunk = true;

                await foreach (var chunk in _service.StreamResponseAsync(prompt))
                {
                    if (firstChunk)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            aiMessage.Segments.Clear(); // remove "Thinking..."
                        });

                        firstChunk = false;
                        _currentSegment = new ChatSegment
                        {
                            IsCode = _isInCodeBlock,
                            Language = _currentLanguage,
                            Text = ""
                        };
                        aiMessage.Segments.Add(_currentSegment);

                    }
                    _currentSegment.Text += chunk;
                    //OnPropertyChanged(nameof(Messages));

                    await Task.Delay(5);


                    //if (firstChunk)
                    //{
                    //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    //    {
                    //        aiMessage.Segments.Clear(); // remove "Thinking..."
                    //    });

                    //    firstChunk = false;
                    //}

                    //await AppendChunk(aiMessage, chunk);
                }
            }
            private readonly StringBuilder _uiBuffer = new();
            private bool _isUpdatingUI = false;
            // =========================
            // STREAM PARSER
            // =========================
            private async Task AppendChunk(ChatMessage message, string chunk)
            {
                foreach (char c in chunk)
                {
                    _tickBuffer.Append(c);

                    // Detect ```
                    if (_tickBuffer.Length >= 3 &&
                        _tickBuffer.ToString().EndsWith("```"))
                    {
                        _tickBuffer.Clear();

                        _isInCodeBlock = !_isInCodeBlock;

                        // Create new segment
                        _currentSegment = new ChatSegment
                        {
                            IsCode = _isInCodeBlock,
                            Language = _isInCodeBlock ? _currentLanguage : null,
                            Text = ""
                        };

                        _currentSegment = new ChatSegment
                        {
                            IsCode = _isInCodeBlock,
                            Language = _currentLanguage,
                            Text = ""
                        };

                        message.Segments.Add(_currentSegment);
                        _currentLanguage = "";
                        _langBuffer.Clear();
                        continue;
                    }

                    // Detect language after ``` (like ```csharp)
                    if (_isInCodeBlock && string.IsNullOrEmpty(_currentLanguage))
                    {
                        _langBuffer.Append(c);

                        if (c == '\n')
                        {
                            _currentLanguage = _langBuffer.ToString().Trim();
                            _langBuffer.Clear();
                            continue; // don't include language in code
                        }
                    }

                    // Create first segment if needed
                    if (_currentSegment == null)
                    {
                        _currentSegment = new ChatSegment
                        {
                            IsCode = false,
                            Text = ""
                        };

                        message.Segments.Add(_currentSegment);

                    }

                    // Append text (UI thread)

                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _currentSegment.Text += c;
                    }, DispatcherPriority.Render);
                    await Task.Delay(3);


                }
            }

            // =========================
            // RESET STATE
            // =========================
            private void ResetStreamState()
            {
                _isInCodeBlock = false;
                _currentLanguage = "";
                _currentSegment = null;
                _tickBuffer.Clear();
                _langBuffer.Clear();
            }

            // =========================
            // COPY COMMAND
            // =========================
            [RelayCommand]
            private void Copy(string text)
            {
                Clipboard.SetText(text);
            }
        }*/



}

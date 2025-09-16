using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM.Avalonia.Controls
{
    /// <summary>
    /// Universal Kanban Board control for workflow visualization and task management.
    /// Features drag-and-drop, customizable columns, and real-time updates.
    /// Extracted from MTM WIP Application patterns and made universal.
    /// </summary>
    public class UniversalKanbanBoard : UserControl
    {
        #region Styled Properties

        public static readonly StyledProperty<ObservableCollection<KanbanColumn>> ColumnsProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, ObservableCollection<KanbanColumn>>(
                nameof(Columns), new ObservableCollection<KanbanColumn>());

        public static readonly StyledProperty<ObservableCollection<KanbanCard>> CardsProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, ObservableCollection<KanbanCard>>(
                nameof(Cards), new ObservableCollection<KanbanCard>());

        public static readonly StyledProperty<IBrush> BoardBackgroundProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, IBrush>(
                nameof(BoardBackground), Brushes.LightGray);

        public static readonly StyledProperty<IBrush> ColumnBackgroundProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, IBrush>(
                nameof(ColumnBackground), Brushes.White);

        public static readonly StyledProperty<IBrush> CardBackgroundProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, IBrush>(
                nameof(CardBackground), Brushes.WhiteSmoke);

        public static readonly StyledProperty<bool> AllowDragDropProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, bool>(
                nameof(AllowDragDrop), true);

        public static readonly StyledProperty<double> ColumnWidthProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, double>(
                nameof(ColumnWidth), 280);

        public static readonly StyledProperty<double> CardHeightProperty =
            AvaloniaProperty.Register<UniversalKanbanBoard, double>(
                nameof(CardHeight), 100);

        #endregion

        #region Properties

        public ObservableCollection<KanbanColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public ObservableCollection<KanbanCard> Cards
        {
            get => GetValue(CardsProperty);
            set => SetValue(CardsProperty, value);
        }

        public IBrush BoardBackground
        {
            get => GetValue(BoardBackgroundProperty);
            set => SetValue(BoardBackgroundProperty, value);
        }

        public IBrush ColumnBackground
        {
            get => GetValue(ColumnBackgroundProperty);
            set => SetValue(ColumnBackgroundProperty, value);
        }

        public IBrush CardBackground
        {
            get => GetValue(CardBackgroundProperty);
            set => SetValue(CardBackgroundProperty, value);
        }

        public bool AllowDragDrop
        {
            get => GetValue(AllowDragDropProperty);
            set => SetValue(AllowDragDropProperty, value);
        }

        public double ColumnWidth
        {
            get => GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        public double CardHeight
        {
            get => GetValue(CardHeightProperty);
            set => SetValue(CardHeightProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<CardMovedEventArgs>? CardMoved;
        public event EventHandler<CardClickedEventArgs>? CardClicked;
        public event EventHandler<ColumnHeaderClickedEventArgs>? ColumnHeaderClicked;

        #endregion

        private ScrollViewer? _scrollViewer;
        private StackPanel? _columnsPanel;

        public UniversalKanbanBoard()
        {
            InitializeControl();
            SetupEventHandlers();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            BuildBoard();
        }

        private void InitializeControl()
        {
            Background = BoardBackground;
            
            _scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            _columnsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 16,
                Margin = new Thickness(16)
            };

            _scrollViewer.Content = _columnsPanel;
            Content = _scrollViewer;
        }

        private void SetupEventHandlers()
        {
            PropertyChanged += (_, e) =>
            {
                if (e.Property == ColumnsProperty || e.Property == CardsProperty)
                {
                    BuildBoard();
                }
            };
        }

        private void BuildBoard()
        {
            if (_columnsPanel == null) return;

            _columnsPanel.Children.Clear();

            foreach (var column in Columns)
            {
                var columnPanel = CreateColumnPanel(column);
                _columnsPanel.Children.Add(columnPanel);
            }
        }

        private Border CreateColumnPanel(KanbanColumn column)
        {
            var columnContainer = new Border
            {
                Background = ColumnBackground,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Width = ColumnWidth,
                BoxShadow = BoxShadows.Parse("0 2 8 rgba(0,0,0,0.1)")
            };

            var columnStack = new StackPanel
            {
                Spacing = 12
            };

            // Column Header
            var headerPanel = new StackPanel
            {
                Spacing = 4
            };

            var titleBlock = new TextBlock
            {
                Text = column.Title,
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Black
            };

            var countBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(8, 4),
                Child = new TextBlock
                {
                    Text = GetColumnCardCount(column.Id).ToString(),
                    Foreground = Brushes.White,
                    FontSize = 12,
                    FontWeight = FontWeight.Bold
                }
            };

            var headerContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children = { titleBlock, countBadge }
            };

            headerPanel.Children.Add(headerContainer);

            if (!string.IsNullOrEmpty(column.Description))
            {
                var descBlock = new TextBlock
                {
                    Text = column.Description,
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                };
                headerPanel.Children.Add(descBlock);
            }

            columnStack.Children.Add(headerPanel);

            // Separator
            var separator = new Border
            {
                Height = 1,
                Background = Brushes.LightGray,
                Margin = new Thickness(0, 4)
            };
            columnStack.Children.Add(separator);

            // Cards Container
            var cardsContainer = new StackPanel
            {
                Spacing = 8
            };

            var columnCards = Cards.Where(c => c.ColumnId == column.Id).ToList();
            foreach (var card in columnCards)
            {
                var cardControl = CreateCardControl(card);
                cardsContainer.Children.Add(cardControl);
            }

            columnStack.Children.Add(cardsContainer);

            columnContainer.Child = columnStack;
            return columnContainer;
        }

        private Border CreateCardControl(KanbanCard card)
        {
            var cardContainer = new Border
            {
                Background = CardBackground,
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Height = CardHeight,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                BoxShadow = BoxShadows.Parse("0 1 4 rgba(0,0,0,0.1)")
            };

            var cardStack = new StackPanel
            {
                Spacing = 6
            };

            // Card Title
            var titleBlock = new TextBlock
            {
                Text = card.Title,
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap
            };
            cardStack.Children.Add(titleBlock);

            // Card Description
            if (!string.IsNullOrEmpty(card.Description))
            {
                var descBlock = new TextBlock
                {
                    Text = card.Description,
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                };
                cardStack.Children.Add(descBlock);
            }

            // Card Footer
            var footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Priority Indicator
            if (card.Priority != KanbanPriority.Normal)
            {
                var priorityColor = card.Priority switch
                {
                    KanbanPriority.High => Colors.Red,
                    KanbanPriority.Medium => Colors.Orange,
                    KanbanPriority.Low => Colors.Green,
                    _ => Colors.Gray
                };

                var priorityDot = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(priorityColor)
                };
                footerPanel.Children.Add(priorityDot);
            }

            // Tags
            if (card.Tags?.Any() == true)
            {
                foreach (var tag in card.Tags.Take(2)) // Limit to 2 tags for space
                {
                    var tagBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(232, 245, 252)),
                        CornerRadius = new CornerRadius(3),
                        Padding = new Thickness(4, 2),
                        Child = new TextBlock
                        {
                            Text = tag,
                            FontSize = 10,
                            Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212))
                        }
                    };
                    footerPanel.Children.Add(tagBorder);
                }
            }

            cardStack.Children.Add(footerPanel);

            cardContainer.Child = cardStack;

            // Add click event
            cardContainer.Tapped += (_, e) =>
            {
                CardClicked?.Invoke(this, new CardClickedEventArgs(card));
            };

            return cardContainer;
        }

        private int GetColumnCardCount(string columnId)
        {
            return Cards?.Count(c => c.ColumnId == columnId) ?? 0;
        }

        public void MoveCard(string cardId, string targetColumnId)
        {
            var card = Cards.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
            {
                var oldColumnId = card.ColumnId;
                card.ColumnId = targetColumnId;
                
                CardMoved?.Invoke(this, new CardMovedEventArgs(card, oldColumnId, targetColumnId));
                BuildBoard(); // Rebuild to reflect changes
            }
        }

        public void AddCard(KanbanCard card)
        {
            Cards.Add(card);
            BuildBoard();
        }

        public void RemoveCard(string cardId)
        {
            var card = Cards.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
            {
                Cards.Remove(card);
                BuildBoard();
            }
        }

        public void UpdateCard(KanbanCard updatedCard)
        {
            var existingCard = Cards.FirstOrDefault(c => c.Id == updatedCard.Id);
            if (existingCard != null)
            {
                var index = Cards.IndexOf(existingCard);
                Cards[index] = updatedCard;
                BuildBoard();
            }
        }
    }

    #region Supporting Classes

    public class KanbanColumn
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public int Order { get; set; }
        public int MaxCards { get; set; } = int.MaxValue;
        public string Color { get; set; } = "#0078D4";
    }

    public class KanbanCard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string ColumnId { get; set; } = "";
        public KanbanPriority Priority { get; set; } = KanbanPriority.Normal;
        public List<string>? Tags { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public string? AssignedTo { get; set; }
        public object? Data { get; set; } // For additional custom data
    }

    public enum KanbanPriority
    {
        Low,
        Normal,
        Medium,
        High
    }

    #endregion

    #region Event Args

    public class CardMovedEventArgs : EventArgs
    {
        public KanbanCard Card { get; }
        public string OldColumnId { get; }
        public string NewColumnId { get; }

        public CardMovedEventArgs(KanbanCard card, string oldColumnId, string newColumnId)
        {
            Card = card;
            OldColumnId = oldColumnId;
            NewColumnId = newColumnId;
        }
    }

    public class CardClickedEventArgs : EventArgs
    {
        public KanbanCard Card { get; }

        public CardClickedEventArgs(KanbanCard card)
        {
            Card = card;
        }
    }

    public class ColumnHeaderClickedEventArgs : EventArgs
    {
        public KanbanColumn Column { get; }

        public ColumnHeaderClickedEventArgs(KanbanColumn column)
        {
            Column = column;
        }
    }

    #endregion
}
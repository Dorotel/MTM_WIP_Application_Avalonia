# GitHub Copilot Instructions: MTM UI Styling and Design System

<details>
<summary><strong>🚨 CRITICAL: AVLN2000 Error Prevention</strong></summary>

**BEFORE applying any styling, ALWAYS consult [avalonia-xaml-syntax.instruction.md](avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

### Key Styling Rules to Prevent AVLN2000:
1. **Use Avalonia Style Selector syntax**: `Selector="Button.primary"` not WPF syntax
2. **Proper resource syntax**: Use `StaticResource` and `DynamicResource` correctly
3. **Correct property names**: Use Avalonia property names, not WPF equivalents
4. **Grid styling**: Never use `Name` properties, use attribute syntax

</details>

<details>
<summary><strong>🎨 MTM Purple Design System</strong></summary>

## MTM Color Palette

### Primary Colors
```xml
<!-- MTM Purple Theme -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#6a0dad"/>           <!-- Primary Purple -->
<SolidColorBrush x:Key="PrimaryLightBrush" Color="#8a2aea"/>      <!-- Light Purple -->
<SolidColorBrush x:Key="PrimaryDarkBrush" Color="#4a0a87"/>       <!-- Dark Purple -->
<SolidColorBrush x:Key="AccentBrush" Color="#ba45ed"/>            <!-- Magenta Accent -->
<SolidColorBrush x:Key="SecondaryBrush" Color="#f0f0f0"/>         <!-- Light Gray -->
```

### Neutral Colors
```xml
<!-- Supporting Colors -->
<SolidColorBrush x:Key="BackgroundBrush" Color="#ffffff"/>        <!-- White Background -->
<SolidColorBrush x:Key="CardBackgroundBrush" Color="#ffffff"/>    <!-- Card Background -->
<SolidColorBrush x:Key="BorderBrush" Color="#e0e0e0"/>            <!-- Light Border -->
<SolidColorBrush x:Key="TextBrush" Color="#333333"/>              <!-- Dark Text -->
<SolidColorBrush x:Key="SubtleBrush" Color="#666666"/>            <!-- Subtle Text -->
```

</details>

<details>
<summary><strong>✅ Styling Best Practices</strong></summary>

### AVLN2000 Prevention in Styling:
- Always use Avalonia Selector syntax: `Selector="Button.primary"`
- Use proper property names that exist in Avalonia
- Test styles with compiled bindings enabled
- Use StaticResource/DynamicResource correctly

### MTM Design Guidelines:
- Consistent use of MTM purple color palette
- 8px base spacing unit (4px, 8px, 16px, 24px, 32px)
- 4px border radius for subtle rounding
- Box shadows for depth and hierarchy
- Responsive design with flexible grids
- Smooth transitions for interactive elements

</details>

---
name: glassmorphism-theme-architect
description: Applies modern Glassmorphism (frosted glass) aesthetics across WPF/XAML and Web platforms. Defines deep color palettes, acrylic/mica translucent layers, and enforces RTL (Arabic) typography (Cairo, Readex Pro).
---

# Glassmorphism Theme Architect Protocol

When instructed to apply a Glassmorphism or modern translucent theme, you MUST follow these directives strictly to achieve premium visual fidelity.

## 1. Core Glass Principles & Accessibility
- **The Glass Layer:** UIs must utilize semi-transparent surfaces (`Opacity 0.05` to `0.25` for light themes, `0.4` to `0.7` for dark themes) layered over a rich, vibrant background mesh or gradient.
- **Specular Highlights (Edges):** Every glass panel MUST have a 1px border that is slightly lighter than the background (`Opacity 0.15` to `0.3`) to simulate light catching the edge of the glass.
- **Contrast & Depth:** Soft drop shadows (`BlurRadius 20-30`, `Opacity 0.15-0.3`) must be applied behind the glass layer to detach it from the background.
- **Text Legibility:** Never place thin text over busy backgrounds without sufficient blur. Use solid white (`#FFFFFF`) or light slate (`#F8FAFC`) for dark-glass text.

## 2. WPF-Specific Implementation (Desktop)
To achieve true glassmorphism in WPF:
1. **Window Setup:** The main window MUST have `WindowStyle="None"`, `AllowsTransparency="True"`, and `Background="Transparent"` to allow custom corner radiuses and background meshes.
2. **Background Mesh:** The root grid should contain an `Image` or a complex `LinearGradientBrush` / `RadialGradientBrush` acting as the colorful backdrop.
3. **The Glass Border (Card):**
   ```xml
   <Border CornerRadius="16" BorderThickness="1.5" Margin="10">
       <Border.Background>
           <SolidColorBrush Color="#0F172A" Opacity="0.65"/> <!-- Deep acrylic fill -->
       </Border.Background>
       <Border.BorderBrush>
           <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
               <GradientStop Color="#50FFFFFF" Offset="0.0" /> <!-- Specular highlight -->
               <GradientStop Color="#05FFFFFF" Offset="1.0" />
           </LinearGradientBrush>
       </Border.BorderBrush>
       <Border.Effect>
           <DropShadowEffect BlurRadius="25" ShadowDepth="5" Opacity="0.25" Direction="270" Color="Black" />
       </Border.Effect>
       <!-- Content Here -->
   </Border>
   ```

## 3. Typography & RTL (Arabic)
- **Primary Arabic Fonts:** `Cairo`, `IBM Plex Sans Arabic`, or `Readex Pro`.
- **Primary Latin Fonts:** `Inter`, `Outfit`, or `Segoe UI Variable`.
- **Scale:** Titles (24-32px, Bold), Headers (18-20px, SemiBold), Body (14px), Captions (12px, muted opacity).
- **RTL Enforcement:** For Arabic applications, always set `FlowDirection="RightToLeft"` at the `Window` or `UserControl` root.

## 4. Web Implementation (Tailwind CSS)
When targeting Web, use standard Backdrop Filter tokens:
- **Glass Container:** `bg-white/10 dark:bg-slate-900/40 backdrop-blur-xl border border-white/20 shadow-2xl rounded-2xl`.
- **Hover Effects:** Increase opacity slightly on hover `hover:bg-white/20 hover:border-white/30 transition-all duration-300`.

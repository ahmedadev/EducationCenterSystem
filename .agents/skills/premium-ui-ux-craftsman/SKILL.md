---
name: premium-ui-ux-craftsman
description: The elite UI/UX design standard for enterprise desktop (WPF/XAML) and web applications. Replaces generic AI aesthetics with bespoke, human-crafted design systems, rich color tokens, depth physics, micro-interactions, and ergonomic data density.
---

# Premium UI/UX Craftsman Protocol (Anti-Generic AI Design)

This skill governs the visual identity, styling architecture, and user experience across all presentation layers in the solution. It strictly outlaws generic AI design clichés and enforces human-centered, production-grade craftsmanship.

---

## 🚫 1. BANNED GENERIC AI TROPES (NEVER USE)
1. **The "AI Purple/Indigo Trap":** Indiscriminate use of `#6366F1`, `#8B5CF6`, or bright gradient pills on everything.
2. **Flat Monotony & Stiff Grids:** Monochromatic flat white boxes with `#E5E7EB` borders and no elevation hierarchy.
3. **Centered Wall-of-Inputs:** Centering forms with arbitrary widths. Forms must be structured into logical cards, 2-column or 3-column ergonomic sections.
4. **Ad-Hoc Inline Colors:** NEVER write inline Hex codes (`#1A365D`, `#F8F9FA`) in user views. Every single brush must reference centralized dynamic resource tokens.
5. **Static Dead UI:** Controls without hover, focus, active, and disabled states.

---

## 🎨 2. THE 5 PILLARS OF BESPOKE ENTERPRISE DESIGN

### 1. Curated Palette & The 60-30-10 Law
- **60% Dominant Neutral Foundation:** Use tinted deep slate/zinc neutrals for dark mode (`#0B0F19`, `#111827`, `#1F2937`) or warm porcelain/cloud for light mode (`#F8FAFC`, `#F1F5F9`, `#FFFFFF`).
- **30% Structural Hierarchy:** Cards, sidebars, headers, table rows, and borders with subtle contrast ratios (`1px` borders with low-opacity alpha, e.g. `rgba(255,255,255,0.08)` or `rgba(15,23,42,0.08)`).
- **10% Purposeful Accent:** One distinct, premium brand accent (e.g., Deep Emerald `#059669`, Warm Amber `#D97706`, or Precision Blue `#0284C7`) reserved exclusively for primary actions and active states.

### 2. Physical Elevation & Light Simulation (Depth)
- Every card and modal must have layered depth:
  - **Ambient Shadow:** Soft, diffused shadow for elevation (`BlurRadius=16`, `Opacity=0.08`, `Direction=270`).
  - **Key Light (Highlight):** Subtle `1px` top/inner border (`#FFFFFF20` in dark mode or `#FFFFFF` in light mode) simulating physical light from above.
  - **Rounded Geometry:** Consistent radii: `6px` for small tags/inputs, `10px` for buttons/cards, `16px` for dialogs/panels.

### 3. Typography Scale & Tabular Numerics
- **Font Stack:** Modern high-legibility geometric sans-serif (e.g., *Segoe UI Variable*, *Inter*, *Dubai* for Arabic RTL).
- **Hierarchy:**
  - `Header Large`: 22-26px, SemiBold/Bold, `-0.5px` character spacing.
  - `Section Title`: 16-18px, SemiBold.
  - `Body Text`: 13-14px, Regular, optical line-height.
  - `Caption/Meta`: 11-12px, Medium, low-contrast neutral.
- **Tabular Numbers:** Monetary amounts, IDs, dates, and counters must use tabular/monospaced numeric alignment.

### 4. High-Efficiency Data Ergonomics (B2B / Center Workflows)
- **Data Grids:**
  - Compact row height (`36px` to `42px`) to maximize visible records.
  - Subtle row hover (`#F1F5F9` or `#1E293B`) without jarring color flashes.
  - Integrated status badges (`Active` = soft green bg + solid green text; `Inactive` = soft gray).
  - Sticky pagination bar with item counter, page size selector, and jump-to-page input.
- **Form Ergonomics:**
  - Group inputs into meaningful semantic cards (e.g., "Personal Info", "Contact Details", "Academic Info").
  - Clear floating or distinct top labels with required asterisk indicators (`*`).
  - Integrated field validation messages directly beneath inputs with smooth visibility triggers.

### 5. Micro-Animations & Fluid States
- **Hover Transitions:** 150ms ease-out transitions on button backgrounds, borders, and shadows.
- **Click Feedback:** Scale transforms (`RenderTransform: Scale 0.98`) on button press for tactile feedback.
- **Skeleton Shimmer:** Smooth skeleton loading placeholders instead of jarring screen freezes.

---

## 🏛️ 3. ARCHITECTURAL WPF ASSET STRUCTURE

All styling assets MUST be organized into `src/EducationCenterSystem.Presentation/Resources/`:
```
src/EducationCenterSystem.Presentation/Resources/
├── Colors.xaml        # Hex palette, DynamicBrushes, Gradients, Status colors
├── Typography.xaml    # Font families, Font sizes, TextBlock styles
├── Metrics.xaml       # Standard padding (4, 8, 12, 16, 24), corner radii
├── Controls/
│   ├── Buttons.xaml   # Primary, Secondary, Danger, Icon-only styles
│   ├── Inputs.xaml    # TextBoxes, ComboBoxes, DatePickers with focus states
│   └── DataGrid.xaml  # Professional data table styles and scrollbars
└── Theme.xaml         # Merged dictionary linking all resource dictionaries
```

---

## ⚡ 4. REVIEW & GATEKEEPING CHECKLIST
Before committing any UI change, verify:
- [ ] Are all colors referenced from `Theme.xaml` via `{DynamicResource ...}`?
- [ ] Does every interactive button have Hover, Pressed, and Disabled visual states?
- [ ] Is RTL (`FlowDirection="RightToLeft"`) handled seamlessly with proper Arabic margins and alignments?
- [ ] Are numeric values formatted with appropriate number group separators (`N0`, `C2`)?
- [ ] Is the UI legible at both 100% and 125%/150% Windows DPI scaling?

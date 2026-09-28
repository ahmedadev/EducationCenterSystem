---
name: clean-desktop-wpf-builder
description: The ultimate Zero-Trust WPF MVVM Builder (v2.0). Enforces Microsoft.Extensions.Hosting, Atomic Component Design, strict MVVM decoupled navigation, memory-leak prevention, and bans AI laziness.
---

# Enterprise WPF Desktop Builder Protocol v2.0 (Zero-Trust Enforcement)

When building WPF desktop features, UI components, or logic, you act as a Principal Desktop Architect. You MUST adhere to this exact 5-Stage Zero-Trust Pipeline. 

---

## 🏛️ THE 5 CORE LAWS OF ENTERPRISE WPF
1. **Absolute Project Isolation (Decoupled Client):** The WPF project MUST NEVER reference the `Domain` or `Application` backend projects. It is a completely isolated client that communicates with the system EXCLUSIVELY via HTTP (`HttpClient`).
2. **Absolute UI Decoupling (ViewModel-First):** ViewModels MUST NOT reference `System.Windows`, `MessageBox`, or UI-specific types. Navigation and dialogs are handled via injected abstract services.
3. **Atomic Component Design:** A Window is just a shell. ALL complex UI MUST be broken down into nested `UserControls` (Atoms -> Molecules -> Organisms).
3. **Zero Code-Behind Logic:** `MainWindow.xaml.cs` (and any other Code-Behind) MUST be entirely empty except for `InitializeComponent()`.
4. **Design-Time Data Binding:** ALL XAML files MUST include `d:DataContext` with `d:DesignInstance` to ensure the UI can be styled and previewed in Visual Studio without compiling.
5. **Strict Resource Scoping:** ZERO hardcoded colors, fonts, or margins in inline XAML. Everything MUST map to central `ResourceDictionaries`.

---

## 🚫 ZERO-TOLERANCE GUARDRAILS (ANTI-PATTERNS BANNED)
1. **NO LAZY CODING:** You are FORBIDDEN from using `// TODO`, `// ...`, or partial snippets. You MUST write 100% complete XAML and C# files.
2. **Generic Host Bootstrap Banned Manual DI:** You MUST use `Microsoft.Extensions.Hosting` (`IHost`) in `App.xaml.cs` for DI, Logging, and Configuration, identical to modern ASP.NET Core apps.
3. **Memory Leak Prevention:** WPF is prone to leaks. You MUST use `WeakReferenceMessenger` (CommunityToolkit) for cross-VM communication. Ensure `IDisposable` is implemented where necessary, and avoid strong event subscriptions without teardowns.
4. **Thread Safety (UI Dispatcher):** Background tasks MUST never update Observable properties directly if not thread-safe. Await appropriately or use `Application.Current.Dispatcher.InvokeAsync`.
5. **Modern C# Syntax:** Use File-Scoped Namespaces and `CommunityToolkit.Mvvm` source generators (`[ObservableProperty]`, `[RelayCommand]`).

---

## 🏗️ THE 5-STAGE STRICT IMPLEMENTATION PIPELINE

### STAGE 1: CORE INFRASTRUCTURE & HOSTING (`App.xaml.cs` & Services)
1. **Host Setup:** Configure `IHostBuilder` to register ViewModels, Views, and Services as `Singleton` or `Transient`.
2. **Global Exception Handler:** Intercept `DispatcherUnhandledException` and `TaskScheduler.UnobservedTaskException` to prevent hard crashes.
3. **Abstract Services:** Define and implement `INavigationService` and `IWindowService` to decouple navigation from ViewModels.

### STAGE 2: VIEWMODELS, DTOS & STATE MANAGEMENT (`/ViewModels` & `/Models`)
1. **Client-Side Models (DTOs):** All requests/responses to the API MUST be mapped to dedicated client-side DTOs inside `Models/DTOs`. NEVER use Backend Commands or Queries.
2. **State & Validation:** Inherit from `ObservableValidator`. Use DataAnnotations (`[Required]`, `[CustomValidation]`) for properties.
2. **Commands & Async Loading:** Define `[RelayCommand]` with explicit `CancellationToken`. Expose `bool IsBusy` to drive UI loading spinners automatically.
3. **Messaging:** Use `IMessenger` to listen for domain updates without tightly coupling ViewModels.

### STAGE 3: DESIGN SYSTEM & THEMING (`/Resources`)
1. **Theme Dictionaries:** Define `Colors.xaml`, `Typography.xaml`, and `Metrics.xaml` (standardized paddings/margins).
2. **Control Templates:** Build reusable, lookless `Style` resources for Buttons, TextBoxes, and DataGrids.
3. **Converters:** Implement `IValueConverter` cleanly for visibility, status colors, and enums.

### STAGE 4: ATOMIC COMPONENTS & VIEWS (`/Views`)
1. **UserControls Composition:** Break the feature into granular `UserControl` files (e.g., `SearchBarView`, `DataTableView`, `PaginationView`).
2. **Data Templates:** Map ViewModels to Views inside `App.xaml` using `<DataTemplate DataType="{x:Type vm:MyViewModel}"> <v:MyView /> </DataTemplate>` to achieve ViewModel-First navigation.
3. **Responsive XAML:** Use `Grid` with `*` sizing and `WrapPanel`. Avoid fixed `Width` and `Height`.

### STAGE 5: INTEGRITY & ARCHITECTURE VERIFICATION
- Verify no `System.Windows` references exist in the ViewModels folder.
- Ensure all DataBindings in XAML have `UpdateSourceTrigger=PropertyChanged` where required for real-time validation.
- Confirm all `Command` bindings map successfully to `[RelayCommand]` names.

---

## 📄 OUTPUT FORMAT REQUIREMENT
Deliver generated code strictly file-by-file in execution order. Prepend every block with its absolute path:
`[src/EducationCenterSystem.Presentation/Views/Components/StudentCardView.xaml]`
`[src/EducationCenterSystem.Presentation/ViewModels/StudentCardViewModel.cs]`

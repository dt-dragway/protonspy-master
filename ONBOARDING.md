# Onboarding: iSpy - Arquitectura y Desarrollo

## 1.0 Preámbulo: Un Contrato de Confianza Digital

Bienvenido a iSpy. Este documento no es solo una guía técnica; es un pacto de confianza. Al asumir su rol, se convierte en el guardián de un ecosistema complejo y en producción. Cada línea de código, cada decisión arquitectónica y cada nueva funcionalidad debe ser implementada con una premisa fundamental: **la estabilidad es sagrada**. Nuestra misión es avanzar sin romper, innovar con responsabilidad y asegurar que la calidad del software sea la base de nuestro progreso.

### 1.2 Su Rol: Arquitecto y Custodio Técnico

Usted asume la doble responsabilidad de **Arquitecto de Software Principal** y **Custodio de la Integridad Técnica** del proyecto. Su rol trasciende la programación; es el garante de que la visión se ejecute con precisión y excelencia. Debe defender la arquitectura contra la entropía, asegurar la coherencia a largo plazo y tomar decisiones que equilibren la innovación con la estabilidad. Su criterio será la última línea de defensa para mantener la robustez y fiabilidad del sistema.

---

## 2. La Visión del Proyecto

iSpy es una solución de software de vigilancia de video de código abierto, potente y extensible. La visión es proporcionar a los usuarios una plataforma robusta, personalizable y accesible para la monitorización de cámaras y micrófonos, la detección de movimiento, la grabación continua y la notificación de eventos en tiempo real. El proyecto busca democratizar la seguridad personal y profesional a través de una herramienta rica en funcionalidades y compatible con una vasta gama de hardware.

## 3. Roadmap de Funcionalidades: Estado y Prioridades

| Categoría | Funcionalidad | Estado | Prioridad | Notas |
| :--- | :--- | :--- | :--- | :--- |
| **Core** | Captura de Video/Audio | ✅ Estable | Alta | Soporte para ONVIF, RTSP, USB, etc. |
| | Detección de Movimiento | ✅ Estable | Alta | Algoritmos configurables. |
| | Grabación y Almacenamiento | ✅ Estable | Alta | Gestión de archivos y cuotas. |
| **Conectividad** | Acceso Remoto (Servidor Web) | ✅ Estable | Media | Requiere optimización de seguridad. |
| | Integración Cloud (Dropbox, G-Drive) | 🟡 En Desarrollo | Baja | Mejorar la fiabilidad de la subida. |
| | Notificaciones (Email, SMS, Push) | ✅ Estable | Media | |
| **Inteligencia** | Reconocimiento de Objetos | 🟡 En Desarrollo | Media | Integración con modelos externos. |
| | Zonas de Detección Avanzadas | ✅ Estable | Alta | Máscaras y áreas de exclusión. |
| **UX/UI** | Modernización de la Interfaz | 🔴 Planeado | Baja | Migrar a una tecnología más moderna. |
| | Asistente de Configuración | ✅ Estable | Media | Simplificar el onboarding de dispositivos. |

## 4. Arquitectura del Software: Principios de Diseño

iSpy está construido sobre una **arquitectura monolítica de escritorio** utilizando **Windows Forms**. Aunque es un diseño clásico, ha demostrado ser robusto y de alto rendimiento para las tareas de procesamiento intensivo que requiere la aplicación.

- **Capa de Presentación (UI):** Compuesta por formularios de Windows Forms (`MainForm.cs` es el núcleo). La interfaz es event-driven y gestiona la interacción del usuario. Se utiliza la librería `MetroFramework` para un aspecto más moderno.
- **Lógica de Aplicación (Core):** La lógica de negocio reside en clases de servicio y manejadores (`MainForm_Configuration.cs`, `MainForm_Media.cs`). Gestiona el ciclo de vida de cámaras, micrófonos, alertas y grabaciones.
- **Capa de Acceso a Datos:** No existe una base de datos relacional. La configuración, los objetos (cámaras, micrófonos) y las listas de archivos se persisten en **archivos XML** (`config.xml`, `objects.xml`, `filelist.xml`). Las clases en el directorio `XML/` gestionan la serialización y deserialización.
- **Capa de Abstracción de Hardware (Sources):** El corazón de la captura de medios. Clases bajo `Sources/` abstraen las diferentes fuentes de video y audio (DirectShow, FFmpeg, VLC, Kinect, etc.), presentando una interfaz unificada al resto de la aplicación.
- **Integraciones Externas:** Componentes como `Webservices.cs`, `ftp.cs`, y las clases en `Cloud/` manejan la comunicación con servicios de terceros.

## 5. Configuración del Entorno de Desarrollo

1.  **IDE:** **Visual Studio 2019** o superior (con soporte para desarrollo de escritorio .NET).
2.  **Framework:** **.NET Framework 4.7.2**. Asegúrese de tener el Developer Pack instalado.
3.  **Control de Versiones:** Git.

## 6. Primeros Pasos: Compilación y Ejecución

1.  Clone el repositorio desde la fuente oficial.
2.  Abra el archivo de solución `iSpy.sln` en Visual Studio.
3.  Restaure los paquetes NuGet. Haga clic derecho en la solución dentro del Explorador de Soluciones y seleccione "Restaurar paquetes NuGet".
4.  Configure la plataforma de compilación a **x86** o **x64** según su sistema operativo y necesidades. La configuración `x86` es generalmente más compatible con librerías de 32 bits.
5.  Compile el proyecto (tecla `F6` o `Build > Build Solution`).
6.  Ejecute la aplicación (tecla `F5` o `Debug > Start Debugging`).

## 7. Gestión de la Base de Datos

Como se mencionó, iSpy no utiliza una base de datos tradicional. Toda la configuración del usuario, la lista de cámaras/micrófonos y sus propiedades se almacenan en archivos XML ubicados en el directorio de datos de la aplicación. La manipulación de estos datos se realiza mediante serialización y deserialización de objetos C#.

## 8. Tecnologías y Librerías Clave

- **Lenguaje:** C#
- **Framework:** .NET Framework 4.7.2, Windows Forms
- **Procesamiento de Video/Imagen:**
    - **AForge.NET:** Librería fundamental para el procesamiento de imágenes y visión por computadora.
    - **FFmpeg.AutoGen:** Wrapper para la librería FFmpeg, utilizado para la decodificación de una amplia variedad de códecs de video.
    - **LibVLCSharp:** Integración con el motor de VLC para la reproducción de streams de red complejos.
- **Procesamiento de Audio:**
    - **NAudio:** Potente librería para la captura, reproducción y manipulación de audio.
- **Interfaz de Usuario:**
    - **MetroFramework:** Aporta un estilo moderno (Metro/Fluent) a la aplicación WinForms.
- **Comunicación y Red:**
    - **Newtonsoft.Json:** Serialización/deserialización de JSON para APIs.
    - **RestSharp:** Cliente HTTP para consumir servicios RESTful.
    - **ONVIF:** Múltiples clases para la comunicación con cámaras IP estándar.
- **Hardware:**
    - **SharpDX.DirectInput:** Para el manejo de Joysticks y control PTZ.
    - **Microsoft.Kinect:** Soporte para cámaras Kinect.

## 9. Guía de Contribución y Flujo de Trabajo

1.  **Fork & Branch:** Realice un fork del repositorio y cree una nueva rama descriptiva para su funcionalidad o corrección (`feature/nueva-camara-xyz` o `fix/error-grabacion`).
2.  **Codificación:** Siga las convenciones de estilo y arquitectura existentes. Priorice la estabilidad.
3.  **Pruebas:** Pruebe exhaustivamente sus cambios en un entorno local. Asegúrese de no introducir regresiones.
4.  **Pull Request:** Envíe un Pull Request al repositorio principal, detallando los cambios realizados y el motivo.
5.  **Revisión de Código:** Su código será revisado por el arquitecto principal. Esté preparado para realizar ajustes.

---

## 10. Bitácora de Continuidad y Estado Actual

**Instrucción:** Esta sección debe ser **reemplazada** en cada actualización significativa. No añada nuevas entradas; sustituya la entrada existente por una nueva que refleje el último estado del proyecto, permitiendo al siguiente desarrollador continuar el trabajo de forma fluida.

-   **Fecha:** 22 de septiembre de 2025
-   **Desarrollador:** Gemini
-   **Resumen de Cambios:**
    -   Se ha creado la versión inicial del documento `ONBOARDING.md`.
    -   Se ha realizado un análisis estático del código para identificar la arquitectura, tecnologías y dependencias del proyecto.
    -   Se ha documentado el proceso de configuración del entorno y compilación inicial.
-   **Próximos Pasos:**
    -   Realizar una revisión funcional de las principales características para validar el estado "Estable" del roadmap.
    -   Investigar y documentar el esquema exacto de los archivos de configuración XML (`objects.xml`, `config.xml`).
    -   Identificar la suite de pruebas existente (si la hay) o definir una estrategia para crearla.

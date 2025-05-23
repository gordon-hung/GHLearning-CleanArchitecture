# GHLearning-CleanArchitecture
[![GitHub Actions GHLearning-CleanArchitecture](https://github.com/gordon-hung/GHLearning-CleanArchitecture/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gordon-hung/GHLearning-CleanArchitecture/actions/workflows/dotnet.yml)  [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/gordon-hung/GHLearning-CleanArchitecture) [![codecov](https://codecov.io/gh/gordon-hung/GHLearning-CleanArchitecture/graph/badge.svg?token=C8QLMXHVIO)](https://codecov.io/gh/gordon-hung/GHLearning-CleanArchitecture)

C# Clean Architecture 是一種設計模式，旨在提高應用程式的可維護性、可測試性和可擴展性。它的主要目的是將商業邏輯和基礎設施分離，使得應用程式的不同層次之間具有高度解耦，從而更容易進行單元測試和維護。

以下是C# Clean Architecture的基本結構：

## 核心層（Core）：
- Entities（實體）：代表業務邏輯的核心對象。
- Use Cases（用例）：應用程式的業務邏輯，負責處理來自使用者的請求並執行相應的邏輯。
- Interfaces（接口）：用來定義服務和資料庫等外部資源的交互，這些接口被用來實現依賴倒置原則（DIP）。
## 應用層（Application）：
- Services（服務）：執行具體的業務邏輯，並通過接口與核心層的實體進行交互。
- DTOs（資料傳輸物件）：在不同層之間傳遞數據的物件，通常用來在API層和業務邏輯層之間傳遞數據。

## 基礎設施層（Infrastructure）：
- 實現應用程式與外部系統（如資料庫、網路服務、檔案系統等）的交互。
- 這一層通常包括資料庫的實現、API客戶端、第三方服務等。

## 介面層（Presentation）：
- 用戶界面（UI）或API端點，接收來自用戶或客戶端的請求，並將請求轉發到應用層。

## Clean Architecture的優點：
- 解耦：核心商業邏輯不依賴於具體的基礎設施或框架，使得業務邏輯和外部依賴分離。
- 可測試性：核心層的邏輯可以單獨進行單元測試，無需關注外部的資源或框架。
- 可擴展性：可以很容易地擴展應用程式，添加新的服務或修改現有功能，而不會影響到核心業務邏輯。

## 架構
### 1. **GHLearning.CleanArchitecture.Application**（應用層）
這一層主要處理具體的業務邏輯，定義了應用程序的用例和業務邏輯。它包含了所有服務層和與業務流程相關的處理邏輯。這層會與 **Core** 層進行交互，執行來自用戶或外部系統的操作。

- **用例（Use Cases）**：處理特定的業務邏輯或應用功能。
- **DTOs（資料傳輸物件）**：用於層之間進行數據傳遞，通常是從 API 請求轉換成對應的內部資料結構。
- **Interfaces**：應用層通常會定義接口（例如 Repository 或 Service 接口），這些接口將由基礎設施層來具體實現。

### 2. **GHLearning.CleanArchitecture.Core**（核心層）
這一層是整個架構的核心，包含最純粹的業務邏輯。這裡的實體（Entities）是應用的核心對象，完全不依賴於任何具體的框架或基礎設施。

- **Entities（實體）**：這些是純粹的業務對象，代表了系統中的關鍵實體（例如，`User`、`Product` 等），並包含業務邏輯。
- **Domain Services（領域服務）**：這些服務包含核心業務邏輯，通常與具體的數據持久化無關，專注於處理業務邏輯。
- **Interfaces**：定義應用層與外部系統（如資料庫、API）交互所需的接口。

### 3. **GHLearning.CleanArchitecture.Infrastructure**（基礎設施層）
這一層處理具體的技術實現，負責與外部系統（如資料庫、第三方服務、API）進行交互。這一層通常會實現接口層（定義在 **Core** 或 **Application** 層中的接口）。

- **資料庫操作**：實現資料存取邏輯，通常通過 ORM（例如 Entity Framework）來進行資料庫交互。
- **API 客戶端**：實現與外部 API 的交互，處理 HTTP 請求和回應。
- **服務整合**：將外部服務集成到應用中，如發送電子郵件、處理檔案上傳等。

### 4. **GHLearning.CleanArchitecture.SharedKernel**（共享內核層）
這一層包含在應用中各層之間共享的公共代碼，這些代碼可能包括常用的工具、輔助類別、共用的庫、擴展方法等。

- **共用代碼**：包括所有多層共用的邏輯或類別，例如日誌記錄、錯誤處理等。
- **跨層通用功能**：例如，驗證邏輯、幫助類、常量等。

### 5. **GHLearning.CleanArchitecture.WebApi**（介面層）
這一層負責處理用戶與應用程序之間的交互，通常通過 API 或 Web 前端來接收請求。這一層將把請求轉發到應用層，並返回結果給用戶。

- **Controller**：負責處理 HTTP 請求並轉發到應用層的相應服務。通常，這些控制器會接收 HTTP 請求、處理輸入驗證，並將請求交給應用層處理。
- **API 端點**：向外部客戶端提供 API 服務，通常是 RESTful API，這些端點會根據需求調用應用層的服務來完成具體操作。
- **API 的中介軟件（Middleware）**：處理身份驗證、授權、日誌記錄等跨越多個請求的通用功能。

---
### 總結：
- **Core** 層包含了最重要的業務邏輯，應該是最純粹的部分。
- **Application** 層處理具體的業務流程，與 **Core** 層協作。
- **Infrastructure** 層處理所有與外部系統的交互，並實現 **Core** 或 **Application** 層所需的接口。
- **WebApi** 層處理用戶輸入，並將其轉交給應用層。

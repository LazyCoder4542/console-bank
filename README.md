# Console Bank - CLI Banking Application
<img src="https://github.com/user-attachments/assets/ec948c5f-c6cb-4440-9604-9452da45c69b" alt="console-bank" width="300" height="200"/>

Console Bank is a command-line banking application with persistent data storage, offering essential banking operations through an intuitive terminal interface.

## Key Features

### Core Functionality
- 💳 **Account creation** with secure credentials
- 🔐 User **login/logout** system
- 💰 **Deposit** and **withdraw** funds
- 🔄 **Transfer** funds between accounts
- 📜 **Transaction history** viewing
- 💾 **Data persistence** using binary storage

### Database Implementation
- ⚡ Binary data storage with indexing for fast retrieval
- 🔍 Index files for quick account lookup
- 💽 Efficient data serialization/deserialization
- 🛡️ Data integrity checks

## Getting Started

### Prerequisites
- NET 8.0+
- No external dependencies required

### Installation
```bash
git clone https://github.com/LazyCoder4542/console-bank.git
cd console-bank
```

### Running the Application
```bash
dotnet run
```

## Usage Examples
```text
====================================
      WELCOME TO CONSOLE BANK       
====================================

1. Login to your account
2. Create a new account
3. Exit

Please enter your choice (1-3): 

```
```text
====================================
       TRANSACTION HISTORY
====================================

Date                    Type                          Amount    Description                          From/To
----------------------------------------------------------------------------------------------------------
7/3/2025 9:32:07 PM     Transfer                     $800.00    ljndf                   Ope 12
7/3/2025 9:28:52 PM     Transfer                      $10.20    $                       Ope 12
7/3/2025 9:19:48 PM     Transfer                     $300.00    Make you no suffe...    Aderinola Opemipo   
7/3/2025 5:42:08 PM     Deposit                $4,265,746.00    fdljfdf                 CONSOLEBANK/DEPOSIT 
7/3/2025 5:42:02 PM     Deposit                $3,455,323.00    fkjclasas               CONSOLEBANK/DEPOSIT 
7/3/2025 5:41:51 PM     Deposit                $4,335,425.00    fdsdsdf                 CONSOLEBANK/DEPOSIT 
7/3/2025 5:41:39 PM     Deposit                    $2,314.00                            CONSOLEBANK/DEPOSIT 
7/3/2025 4:33:38 PM     Deposit                       $12.00    Initial Amount          CONSOLEBANK/DEPOSIT 


Press Enter
```

## Database Design
The system uses a dual-file storage approach for efficiency and reliability:

```
data/
├── account.data      # Binary storage of account records
├── account.index     # Index for fast account lookup
├── transaction.data  # Transaction history storage
└── ...               # Others
```

### Key Implementation Details:
- **Indexing**: O(1) account lookup using account number hashing
- **Data Recovery**: Journaling mechanism for write operations
- **Storage Optimization**: Compact binary format minimizes disk usage

## Future Improvements

### Enhanced Banking Features
- 💵 Multiple account support per user
- 🏦 Savings accounts with interest calculation
- 📱 Two-factor authentication
- 📈 Financial reports and analytics

### Database Enhancements
- 🚀 Concurrent access with threading support
- 🧹 Atomic transaction handling
- 🧩 Record versioning for data integrity
- 🔄 Background index optimization
- 🗑️ Secure deletion with tombstoning

### User Experience
- 🌈 Colorful interface with rich text
- 📊 ASCII-based financial charts
- 📋 Export transaction history (CSV/PDF)
- 🌐 Remote access via TCP/IP

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a pull request

## License
Distributed under the MIT License. See `LICENSE` for more information.

---

**Console Bank** - Bringing banking to your terminal since 2025  
*Simple • Secure • Efficient*

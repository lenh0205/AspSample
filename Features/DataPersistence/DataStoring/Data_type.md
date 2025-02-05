
# String

In SQL Server, the most common data types used to store string values are:

VARCHAR(n) – Variable-length non-Unicode string (max n characters)

Example: VARCHAR(50)
Best for storing text that does not require Unicode characters.
More space-efficient compared to NVARCHAR if Unicode support is not needed.
NVARCHAR(n) – Variable-length Unicode string (max n characters)

Example: NVARCHAR(100)
Stores Unicode characters (supports multiple languages).
Uses twice the storage space per character compared to VARCHAR.
CHAR(n) – Fixed-length non-Unicode string (exactly n characters)

Example: CHAR(10)
Best for storing fixed-length text (e.g., codes, abbreviations).
Uses more space than VARCHAR if the string varies in length.
NCHAR(n) – Fixed-length Unicode string (exactly n characters)

Example: NCHAR(20)
Similar to CHAR, but for Unicode data.
TEXT (Deprecated) – Variable-length non-Unicode large text

Replaced by VARCHAR(MAX)
NTEXT (Deprecated) – Variable-length Unicode large text

Replaced by NVARCHAR(MAX)
VARCHAR(MAX) – Variable-length large text (up to 2GB)

Used when storing very long text (e.g., documents, logs).
NVARCHAR(MAX) – Variable-length large Unicode text (up to 2GB)

Used for very long Unicode text.
Which one should you use?
Use VARCHAR(n) if you don’t need Unicode and the length varies.
Use NVARCHAR(n) if you need Unicode support.
Use VARCHAR(MAX) or NVARCHAR(MAX) for very large text storage.
Use CHAR(n) or NCHAR(n) if the length is fixed.
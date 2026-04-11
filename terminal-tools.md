  
## Windows Built-in Tools

### Native Windows 11 CLI Tools (Pre-installed)


| Tool           | Purpose                                           | Equivalent |
| :------------- | :------------------------------------------------ | :--------- |
| **curl.exe**   | Transfers data to/from servers (HTTP, HTTPS, FTP) | curl       |
| **tar.exe**    | Extracts and creates archives (.zip, .tar, .gz)   | tar        |
| **ssh.exe**    | Securely connects to remote servers               | ssh        |
| **findstr**    | Searches for specific text strings in files       | grep       |
| **fc**         | Compares two files and displays differences       | diff       |
| **tasklist**   | Displays all currently running processes          | ps         |
| **taskkill**   | Terminates a process by name or PID               | kill       |
| **systeminfo** | Shows detailed OS and hardware configuration      | uname -a   |
| **netstat**    | Displays active network connections and ports     | netstat    |
| **nslookup**   | Queries DNS to find IP addresses or domains       | dig        |
| **whoami**     | Displays the current logged-in user and groups    | whoami     |
| **sort**       | Sorts lines of text alphabetically or numerically | sort       |
| **more**       | Paginates long text output for terminal viewing   | less       |
| **pathping**   | Traces network paths and reports packet loss      | mtr        |



### Portable Binary Tools (Single-File Executables)

These tools do not require an installation.  They are a single binary that you can download and put on your computer to work within a locked-down enterprise environment.

| Tool | Purpose | Description |
| :--- | :--- | :--- |
| **bat** | Syntax Highlighting | A `cat` clone with syntax highlighting and Git integration for viewing code in the terminal. |
| **fd-find** | File Search | A simple, fast, and user-friendly alternative to `find` that ignores hidden/git directories by default. |
| **fzf** | Fuzzy Finder | An interactive command-line filter that lets you visually search through lists of files, history, or processes. |
| **jq** | JSON Processor | A lightweight and flexible command-line tool for slicing, filtering, and transforming JSON data. |
| **ripgrep** | Text Search | An extremely fast search tool (replaces `grep` or `findstr`) that recursively searches directories for a regex pattern. |
| **zoxide** | Smart Navigation | A smarter `cd` command that remembers your most frequent directories for rapid jumping. |
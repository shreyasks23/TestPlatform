using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Security;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence;

internal static class SeedCatalog
{
    private static readonly Guid UserId1 = Guid.Parse("00000000-0000-0000-0000-900000000001");
    private static readonly Guid UserId2 = Guid.Parse("00000000-0000-0000-0000-900000000002");
    private static readonly Guid UserId3 = Guid.Parse("00000000-0000-0000-0000-900000000003");
    private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-900000000004");
    private static readonly Guid TestId1 = Guid.Parse("00000000-0000-0000-0000-910000000001");
    private static readonly Guid TestId2 = Guid.Parse("00000000-0000-0000-0000-910000000002");
    public const string AdminEmail = "shreyasks@mail.com";
    public const string AdminPassword = "P@SSw0rd";

    public static IReadOnlyList<User> CreateSampleUsers(IPasswordHasher passwordHasher)
        =>
        [
            CreateUser(UserId1, "ada@example.com", "Ada", "Lovelace", passwordHasher),
            CreateUser(UserId2, "grace@example.com", "Grace", "Hopper", passwordHasher),
            CreateUser(UserId3, "alan@example.com", "Alan", "Turing", passwordHasher)
        ];

    public static User CreateAdminUser(IPasswordHasher passwordHasher)
    {
        var adminUser = new User(AdminUserId, AdminEmail, "Shreyas", "K S", "seed-admin-hash", AuthConstants.Roles.Admin);
        adminUser.SetPasswordHash(passwordHasher.HashPassword(adminUser, AdminPassword));
        return adminUser;
    }

    public static IReadOnlyList<Question> CreateQuestions()
    {
        var definitions = new (string Text, string[] Options, int CorrectIndex, string Explanation)[]
        {
            ("What is the average-case time complexity of binary search on a sorted array?", ["O(1)", "O(log n)", "O(n)", "O(n log n)"], 1, "Binary search halves the search space on each comparison, leading to logarithmic time."),
            ("Which data structure uses FIFO ordering?", ["Stack", "Queue", "Heap", "Tree"], 1, "A queue processes elements in first-in, first-out order."),
            ("Which traversal visits left subtree, root, then right subtree in a binary tree?", ["Preorder", "Inorder", "Postorder", "Level-order"], 1, "Inorder traversal is defined as left, root, right."),
            ("Which sorting algorithm is typically stable in its standard form?", ["Heap sort", "Quick sort", "Merge sort", "Selection sort"], 2, "Merge sort preserves equal-element order when implemented with stable merging."),
            ("What is the worst-case time complexity of quicksort?", ["O(log n)", "O(n)", "O(n log n)", "O(n^2)"], 3, "Poor pivot choices can create highly unbalanced partitions, yielding quadratic time."),
            ("Which graph algorithm finds shortest paths from one source when edge weights are non-negative?", ["Kruskal", "Dijkstra", "Prim", "Floyd-Warshall"], 1, "Dijkstra's algorithm is designed for single-source shortest paths with non-negative weights."),
            ("What does a hash table rely on for expected O(1) lookups?", ["Balanced partitions", "Pointer arithmetic", "A good hash distribution", "Recursive traversal"], 2, "Uniform hash distribution keeps collisions low and expected lookup time constant."),
            ("Which data structure is most appropriate for implementing DFS iteratively?", ["Queue", "Stack", "Heap", "Array list"], 1, "Depth-first search uses a stack to track the next node to visit."),
            ("Which algorithmic technique solves overlapping subproblems by storing prior results?", ["Greedy", "Divide and conquer", "Dynamic programming", "Backtracking"], 2, "Dynamic programming caches subproblem answers and reuses them."),
            ("What is the space complexity of merge sort on arrays in its typical implementation?", ["O(1)", "O(log n)", "O(n)", "O(n^2)"], 2, "Merge sort usually allocates auxiliary storage proportional to the input size."),
            ("What is a process in an operating system?", ["A compiled source file", "A program in execution", "A block of memory only", "A CPU scheduling policy"], 1, "A process is an active execution instance of a program."),
            ("Which scheduler decision metric aims to minimize average waiting time but risks starvation?", ["Round robin", "First come first served", "Shortest job first", "Rate-monotonic"], 2, "Shortest job first reduces average waiting time but can starve long jobs."),
            ("What is the main purpose of virtual memory?", ["Increase CPU frequency", "Provide logical address abstraction and larger addressable space", "Eliminate context switches", "Encrypt RAM"], 1, "Virtual memory isolates processes and allows memory to appear larger than physical RAM."),
            ("What problem is solved by a mutex?", ["Address translation", "Mutual exclusion in critical sections", "Disk fragmentation", "Cache coherency only"], 1, "A mutex ensures only one thread enters a critical section at a time."),
            ("Which OS concept describes deadlock prevention by denying at least one Coffman condition?", ["Starvation", "Spooling", "Mutual exclusion", "Deadlock avoidance"], 3, "Preventing or avoiding deadlock means ensuring the necessary conditions cannot all hold simultaneously."),
            ("What happens during a context switch?", ["A file is encrypted", "The CPU switches from one process or thread state to another", "Main memory is formatted", "A packet is routed"], 1, "Context switching saves one execution state and restores another."),
            ("What is paging?", ["A CPU pipeline stage", "Dividing physical memory into variable-sized chunks", "Mapping fixed-size virtual pages to physical frames", "Writing logs to disk"], 2, "Paging uses fixed-size pages and frames for memory management."),
            ("Which storage scheduling algorithm can reduce arm movement by servicing nearby requests first?", ["FCFS", "SCAN", "Round robin", "Lottery scheduling"], 1, "SCAN moves the disk arm in one direction, servicing requests along the way."),
            ("Which statement about threads within a process is correct?", ["Threads have separate address spaces", "Threads share the same address space", "Threads cannot run concurrently", "Threads always map to CPUs one-to-one"], 1, "Threads in the same process share memory and resources."),
            ("What is the primary purpose of a page replacement algorithm?", ["Choose which CPU core to disable", "Choose which memory page to evict from RAM", "Choose which file to compress", "Choose which NIC to route through"], 1, "Page replacement decides which resident page leaves memory when a new page must be loaded."),
            ("Which normal form removes partial dependency on a composite key?", ["1NF", "2NF", "3NF", "BCNF"], 1, "Second normal form eliminates partial dependencies on part of a composite key."),
            ("What does ACID consistency primarily mean?", ["All transactions are read-only", "A transaction moves the database from one valid state to another", "Indexes are always rebuilt", "Queries are always cached"], 1, "Consistency means integrity constraints remain satisfied across transactions."),
            ("Which SQL clause groups rows that share values in specified columns?", ["ORDER BY", "HAVING", "GROUP BY", "UNION"], 2, "GROUP BY forms row groups for aggregate calculations."),
            ("Which index structure is commonly used by relational databases for range queries?", ["B-tree", "Hash index", "Bitmap frame", "Trie array"], 0, "B-trees preserve sorted order and support efficient range scans."),
            ("What is a primary key?", ["A nullable column used for sorting", "A column or set of columns uniquely identifying each row", "Any indexed column", "A foreign key target must be text"], 1, "A primary key uniquely identifies a row and is non-null."),
            ("Which join returns only rows that match in both tables?", ["LEFT JOIN", "RIGHT JOIN", "FULL OUTER JOIN", "INNER JOIN"], 3, "INNER JOIN keeps rows that satisfy the join predicate in both tables."),
            ("What anomaly is reduced by normalization?", ["CPU cache miss anomaly", "Insertion, update, and deletion anomalies", "Socket timeout anomaly", "Hash collision anomaly"], 1, "Normalization reduces redundancy-related data anomalies."),
            ("What does COMMIT do in SQL?", ["Cancels a transaction", "Makes transaction changes durable", "Drops a table", "Creates an index"], 1, "COMMIT finalizes the transaction so its changes persist."),
            ("What isolation issue allows one transaction to read another's uncommitted changes?", ["Phantom read", "Dirty read", "Lost update", "Write skew"], 1, "Dirty reads occur when uncommitted data becomes visible."),
            ("Which SQL aggregate returns the number of rows?", ["SUM", "COUNT", "AVG", "MAX"], 1, "COUNT aggregates rows or non-null values depending on syntax."),
            ("Which protocol provides reliable, connection-oriented transport?", ["IP", "UDP", "TCP", "ICMP"], 2, "TCP establishes a connection and provides ordered, reliable delivery."),
            ("Which OSI layer is responsible for logical addressing and routing?", ["Data link", "Network", "Transport", "Presentation"], 1, "The network layer handles packet routing across networks."),
            ("What is the purpose of DNS?", ["Encrypt packets", "Translate domain names to IP addresses", "Allocate CPU time", "Compress files"], 1, "DNS maps human-readable names to network addresses."),
            ("Which HTTP method is typically idempotent and used to retrieve data?", ["POST", "PATCH", "GET", "CONNECT"], 2, "GET retrieves resources and repeating it should not change server state."),
            ("What is the role of ARP on a local network?", ["Resolve MAC addresses from IP addresses", "Encrypt TCP streams", "Assign public IP addresses", "Compress Ethernet frames"], 0, "ARP maps IP addresses to link-layer MAC addresses on a LAN."),
            ("Which term describes the maximum amount of data that can be transferred per unit time?", ["Latency", "Jitter", "Bandwidth", "Hop count"], 2, "Bandwidth measures potential transfer capacity over time."),
            ("What is a subnet mask used for?", ["Encrypt DNS packets", "Divide an IP address into network and host portions", "Store router passwords", "Detect physical cable failures"], 1, "A subnet mask defines which bits belong to the network prefix."),
            ("What does HTTPS add to HTTP?", ["Packet fragmentation", "TLS-based confidentiality and integrity", "UDP transport", "Automatic caching"], 1, "HTTPS is HTTP carried over TLS for secure communication."),
            ("Which network device primarily forwards packets between different IP networks?", ["Switch", "Hub", "Router", "Repeater"], 2, "Routers inspect network-layer addresses and forward packets across networks."),
            ("What is the primary purpose of the three-way handshake in TCP?", ["Choose a DNS server", "Establish connection state and sequence numbers", "Compress data", "Route around congestion"], 1, "The handshake synchronizes both sides before data transfer begins."),
            ("Which SOLID principle says a class should have only one reason to change?", ["Single Responsibility Principle", "Open/Closed Principle", "Liskov Substitution Principle", "Interface Segregation Principle"], 0, "SRP keeps each class focused on one responsibility."),
            ("What does encapsulation in OOP achieve?", ["It forces inheritance", "It hides internal state behind controlled access", "It removes methods", "It guarantees immutability"], 1, "Encapsulation protects invariants by exposing controlled behavior."),
            ("Which relationship best describes 'a Car has an Engine' in design?", ["Inheritance", "Composition", "Polymorphism", "Overloading"], 1, "Composition models a whole-part relationship."),
            ("What is polymorphism?", ["Storing data in multiple tables", "Using one interface with different underlying implementations", "A type of SQL join", "A CPU scheduling algorithm"], 1, "Polymorphism allows different types to respond through a shared contract."),
            ("Which pattern centralizes object creation behind a dedicated method or class?", ["Decorator", "Factory", "Observer", "Adapter"], 1, "Factory abstractions encapsulate creation logic."),
            ("Why are interfaces useful?", ["They guarantee shared fields", "They define behavior contracts without implementation details", "They always improve runtime speed", "They replace databases"], 1, "Interfaces describe capabilities independently of concrete implementations."),
            ("What is recursion?", ["Calling another process over the network", "A function calling itself with smaller subproblems", "Iterating with a queue only", "Allocating stack memory manually"], 1, "Recursion solves a problem by reducing it to smaller instances of itself."),
            ("Which C# keyword prevents a class from being inherited?", ["virtual", "sealed", "override", "readonly"], 1, "The sealed keyword forbids inheritance."),
            ("What does Big-O notation describe?", ["Exact runtime in milliseconds", "Asymptotic upper bound growth rate", "Memory address offsets", "Compiler optimization levels"], 1, "Big-O describes how runtime or space grows as input size increases."),
            ("What is the difference between stack and heap memory in many languages?", ["Stack is always slower than heap", "Stack is typically for call frames; heap is for dynamic allocation", "Heap cannot store objects", "Stack exists only on disk"], 1, "Call frames are commonly stored on the stack, while dynamically allocated objects live on the heap."),
            ("Why is immutability often useful in concurrent systems?", ["It increases network bandwidth", "It reduces shared-state mutation bugs", "It eliminates all locking needs", "It makes code shorter automatically"], 1, "Immutable data avoids many race conditions because it cannot be changed after creation.")
        };

        var questions = new List<Question>(definitions.Length);

        for (var index = 0; index < definitions.Length; index++)
        {
            var definition = definitions[index];
            var questionNumber = index + 1;
            var options = definition.Options
                .Select((text, optionIndex) => new Option(
                    Guid.Parse($"00000000-0000-0000-0000-{questionNumber * 10 + optionIndex + 1:000000000000}"),
                    text))
                .ToArray();

            var question = new Question(
                Guid.Parse($"00000000-0000-0000-0000-{questionNumber:000000000000}"),
                definition.Text,
                options,
                options[definition.CorrectIndex].Id,
                definition.Explanation);
            questions.Add(question);
        }

        return questions;
    }

    public static IReadOnlyList<Test> CreateTests(IReadOnlyList<Question> questions)
    {
        var fundamentals = new Test(TestId1, "Computer Science Core Assessment");
        foreach (var question in questions.Take(5))
            fundamentals.AddQuestion(question.Id);

        var systemsAndDesign = new Test(TestId2, "General CS Review Assessment");
        foreach (var question in questions.Skip(5).Take(5))
            systemsAndDesign.AddQuestion(question.Id);

        return [fundamentals, systemsAndDesign];
    }

    private static User CreateUser(Guid id, string email, string firstName, string lastName, IPasswordHasher passwordHasher)
    {
        var user = new User(id, email, firstName, lastName, "sample-user-hash", AuthConstants.Roles.Admin);
        user.SetPasswordHash(passwordHasher.HashPassword(user, Guid.NewGuid().ToString("N")));
        return user;
    }
}

# Project 1: DevShelf API — Postman Mastery Guide
### Stack: React + .NET | Tier 1 → Tier 2 Skills

> **How to use this file:** Don't read it all at once. Open the section that matches what you're building *right now*. Every Postman skill is introduced at the exact moment your project creates the pain that skill solves.

---

## What You're Building

A developer bookshelf API where users can:
- Register and log in
- Browse and search books
- Add books to a personal reading list
- Mark books as read / in-progress / want-to-read
- View their profile and reading stats

**Tech:** .NET Web API backend + React frontend + SQL Server database

---

## Project Phases & Postman Skills Unlocked

| Phase | Feature You Build | Postman Skill You Learn |
|---|---|---|
| 1 | Project setup + health check | Environments, variables |
| 2 | Register + Login endpoints | Test assertions, pre-request scripts |
| 3 | Protected routes (JWT) | Request chaining, Bearer auth |
| 4 | Books CRUD | Collection runner |
| 5 | Reading list | Data-driven testing |
| 6 | OpenAPI docs | Schema validation |
| 7 | Deploy to staging | Newman + GitHub Actions CI/CD |

---

## Phase 1 — Project Setup

### What to build in .NET

```csharp
// Program.cs — minimal start
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
```

```csharp
// Controllers/HealthController.cs
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new { status = "healthy", timestamp = DateTime.UtcNow, version = "1.0.0" });
}
```

### Postman Skill: Environments & Variables

**The pain you're about to feel without this:**
You'll have `http://localhost:5259` hardcoded in 30 requests. Then you deploy to staging and have to change all 30. Then production. Don't do it.

**Set this up RIGHT NOW before writing a single request:**

1. Open Postman → click **Environments** (left sidebar)
2. Click **+** to create new environment
3. Name it `DevShelf - Local`
4. Add these variables:

| Variable | Initial Value | Current Value |
|---|---|---|
| `base_url` | `http://localhost:5259` | `http://localhost:5259` |
| `token` | *(leave empty)* | *(leave empty)* |
| `user_id` | *(leave empty)* | *(leave empty)* |

5. Click **Save**
6. Create a second environment: `DevShelf - Staging`
   - `base_url` = `https://devshelf-staging.azurewebsites.net` (your future Azure URL)

> **Critical rule:** Never put real secrets in **Initial Value** — it syncs to Postman cloud and your whole team can see it. Secrets go in **Current Value** only.

**Your first request using variables:**

```
GET {{base_url}}/api/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2026-04-24T10:00:00Z",
  "version": "1.0.0"
}
```

**Write your first test — open the Tests tab:**

```javascript
pm.test("Health check returns 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Status is healthy", function () {
    const json = pm.response.json();
    pm.expect(json.status).to.equal("healthy");
});

pm.test("Response time under 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

## Phase 2 — Auth Endpoints

### What to build in .NET

```csharp
// Models/User.cs
public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// DTOs/AuthDtos.cs
public record RegisterRequest(string UserName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(Guid Id, string UserName, string Email, string Token);
```

```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result is null)
            return BadRequest(new { message = "Email already in use" });
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials" });
        return Ok(result);
    }
}
```

### Postman Skill: Pre-Request Scripts

**The pain:** You keep hitting `400 Bad Request — Email already in use` because you're testing with the same email over and over.

**Solution — Pre-request Script on your Register request:**

Click the **Pre-request Script** tab on your Register request:

```javascript
// Generate a unique email and username every time you send
const timestamp = Date.now();
pm.environment.set("unique_email", `user${timestamp}@devshelf.com`);
pm.environment.set("unique_username", `devuser${timestamp}`);
```

**Your Register request body:**

```json
{
  "userName": "{{unique_username}}",
  "email": "{{unique_email}}",
  "password": "TestPass123!"
}
```

Every send creates a fresh user. No more duplicate email errors.

### Postman Skill: Test Assertions (The Real Deal)

Don't just test for `200 OK`. That tells you almost nothing. Here's what a complete test suite looks like for your Register endpoint:

**Tests tab on Register:**

```javascript
// 1. Status code
pm.test("Register returns 200 OK", function () {
    pm.response.to.have.status(200);
});

// 2. Response structure - does the shape match what you promised?
pm.test("Response has all required fields", function () {
    const json = pm.response.json();
    pm.expect(json).to.have.property("id");
    pm.expect(json).to.have.property("userName");
    pm.expect(json).to.have.property("email");
    pm.expect(json).to.have.property("token");
});

// 3. Data integrity - did we get back what we sent?
pm.test("Email in response matches registration email", function () {
    const json = pm.response.json();
    const sentEmail = pm.environment.get("unique_email");
    pm.expect(json.email).to.equal(sentEmail);
});

// 4. Token shape - is it a real JWT?
pm.test("Token is valid JWT format", function () {
    const json = pm.response.json();
    pm.expect(json.token.split(".").length).to.equal(3);
});

// 5. Token is not empty
pm.test("Token has content", function () {
    const json = pm.response.json();
    pm.expect(json.token.length).to.be.greaterThan(50);
});

// 6. ID is a valid GUID
pm.test("User ID is a valid GUID", function () {
    const json = pm.response.json();
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(json.id).to.match(guidRegex);
});

// 7. Performance
pm.test("Response time under 1000ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});
```

**Tests tab on Login:**

```javascript
pm.test("Login returns 200 OK", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has token", function () {
    const json = pm.response.json();
    pm.expect(json).to.have.property("token");
});

pm.test("Token is valid JWT", function () {
    const json = pm.response.json();
    pm.expect(json.token.split(".").length).to.equal(3);
});

// Test what SHOULD fail
pm.test("Invalid login returns 401", function () {
    // This test is on a SEPARATE request with wrong credentials
    pm.response.to.have.status(401);
});
```

**Create a "Login - Wrong Password" request** and test the failure case too. Untested error paths are where bugs live.

---

## Phase 3 — JWT Auth + Protected Routes

### What to build in .NET

```csharp
// Program.cs additions
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
// ...
app.UseAuthentication();
app.UseAuthorization();
```

```csharp
// Controllers/ProfileController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]  // <-- requires valid JWT
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(new { userId, email, fetchedAt = DateTime.UtcNow });
    }
}
```

### Postman Skill: Request Chaining — Save Token Automatically

**The pain:** You login, copy the token manually, paste it into the next request. Then the token expires and you do it again. This is what separates people who test from people who waste time.

**On your Login request Tests tab, ADD this at the bottom:**

```javascript
// Save token and user ID automatically after every login
pm.test("Save auth token to environment", function () {
    const json = pm.response.json();
    pm.environment.set("token", json.token);
    pm.environment.set("user_id", json.id);
    console.log("Token saved. User ID:", json.id);
});
```

**Now on your Profile request — Auth tab:**
- Select **Bearer Token**
- Token field: `{{token}}`

Postman fills this automatically. You never touch a token manually again.

**Test your protected route:**

```javascript
pm.test("Profile returns 200 for authenticated user", function () {
    pm.response.to.have.status(200);
});

pm.test("Profile contains userId", function () {
    const json = pm.response.json();
    pm.expect(json).to.have.property("userId");
});

// Test that unauthenticated access is rejected
// Create a separate "Profile - No Token" request with no auth header
pm.test("Unauthenticated request returns 401", function () {
    pm.response.to.have.status(401);
});
```

---

## Phase 4 — Books CRUD

### What to build in .NET

```csharp
// Models/Book.cs
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int PublishedYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

```csharp
// Controllers/BooksController.cs
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? genre,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // implementation
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetById(Guid id) { }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Book>> Create(CreateBookRequest request) { }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Book>> Update(Guid id, UpdateBookRequest request) { }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id) { }
}
```

### Postman Skill: Collection Runner

**The pain:** You have 12 requests now. You change one endpoint and have no idea if you broke anything else.

**Organize your collection like this:**

```
DevShelf API/
├── Health/
│   └── GET Health Check
├── Auth/
│   ├── POST Register
│   ├── POST Login
│   └── POST Login - Wrong Password (expects 401)
├── Profile/
│   ├── GET Profile (authenticated)
│   └── GET Profile - No Token (expects 401)
└── Books/
    ├── GET All Books
    ├── GET Book by ID
    ├── GET Books - Search
    ├── POST Create Book
    ├── PUT Update Book
    └── DELETE Book
```

**Run the whole thing:**
1. Click the **...** next to your collection name
2. Select **Run collection**
3. Choose `DevShelf - Local` environment
4. Click **Run DevShelf API**

You should see:

```
Health Check           ✓  200 OK   (3/3 tests)
Register               ✓  200 OK   (7/7 tests)
Login                  ✓  200 OK   (4/4 tests)
Login - Wrong Pass     ✓  401      (1/1 tests)
Profile (auth)         ✓  200 OK   (2/2 tests)
Profile (no token)     ✓  401      (1/1 tests)
GET All Books          ✓  200 OK   (3/3 tests)
...

Total: 28/28 tests passed
```

Run this before EVERY commit. If anything fails, you don't push.

**Save the book ID for chained requests:**

On your POST Create Book Tests tab:

```javascript
pm.test("Book created successfully", function () {
    pm.response.to.have.status(201);
});

pm.test("Save book ID for next requests", function () {
    const json = pm.response.json();
    pm.environment.set("book_id", json.id);
});
```

Now GET, PUT, DELETE can all use `{{book_id}}` automatically.

---

## Phase 5 — Reading List

### What to build in .NET

```csharp
// Models/ReadingListItem.cs
public class ReadingListItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public ReadingStatus Status { get; set; }
    public int? CurrentPage { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
}

public enum ReadingStatus
{
    WantToRead,
    InProgress,
    Completed
}
```

### Postman Skill: Data-Driven Testing

**The pain:** Your reading list endpoint only gets tested with one happy-path input. What happens with edge cases? Invalid status strings? Page numbers higher than the book's total? Zero? Negative?

**Create a CSV file** called `reading-list-test-data.csv`:

```csv
status,currentPage,expectedStatus,testDescription
WantToRead,,200,Valid want-to-read with no page
InProgress,47,200,Valid in-progress with page number
Completed,,200,Valid completed status
InvalidStatus,,400,Bad status string should return 400
InProgress,-1,400,Negative page number should fail
InProgress,0,400,Zero page number should fail
InProgress,99999,400,Unrealistically high page should fail
,50,400,Missing status should fail
```

**Your test script for this data-driven run:**

```javascript
pm.test(`Test: ${pm.iterationData.get("testDescription")}`, function () {
    const expectedStatus = pm.iterationData.get("expectedStatus");
    pm.response.to.have.status(parseInt(expectedStatus));
});

pm.test("Response shape is correct on success", function () {
    if (pm.response.code === 200) {
        const json = pm.response.json();
        pm.expect(json).to.have.property("id");
        pm.expect(json).to.have.property("status");
    }
});
```

**In Collection Runner:**
1. Select your "Add to Reading List" request
2. Click **Select File** under Data
3. Choose your CSV file
4. Run — Postman fires 8 requests automatically, one per row

This is how you find bugs that manual testing never catches.

---

## Phase 6 — OpenAPI + Schema Validation

### Add Swagger annotations in .NET

```csharp
// appsettings for Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DevShelf API",
        Version = "v1",
        Description = "Developer bookshelf management API"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});
```

### Postman Skill: Schema Validation

**Import your OpenAPI spec into Postman:**
1. Click **Import** in Postman
2. Select **Link** and paste `http://localhost:5259/swagger/v1/swagger.json`
3. Postman generates a collection from your spec automatically

**Add schema validation to every endpoint test:**

First, define your schema in a pre-request script or inline:

```javascript
// In Tests tab — validate Book response shape
const bookSchema = {
    type: "object",
    required: ["id", "title", "author", "isbn", "genre"],
    properties: {
        id: { type: "string", format: "uuid" },
        title: { type: "string", minLength: 1 },
        author: { type: "string", minLength: 1 },
        isbn: { type: "string" },
        publishedYear: { type: "number" },
        genre: { type: "string" }
    }
};

pm.test("Response matches Book schema", function () {
    const json = pm.response.json();
    pm.expect(tv4.validate(json, bookSchema)).to.be.true;
});
```

**Why this matters:** Your frontend developer is consuming this API. If your response shape changes without them knowing, their app breaks silently. Schema validation catches that contract break before it reaches them.

---

## Phase 7 — Deploy + CI/CD

### Postman Skill: Newman + GitHub Actions

**Install Newman:**

```bash
npm install -g newman
```

**Export your collection:**
1. Click **...** on your collection
2. **Export** → Collection v2.1
3. Save as `devshelf.postman_collection.json`

**Export your environment:**
1. Click **...** on `DevShelf - Local`
2. **Export**
3. Save as `devshelf-local.postman_environment.json`

**Create `.github/workflows/api-tests.yml`:**

```yaml
name: DevShelf API Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Start API
        run: |
          dotnet run --project src/DevShelf.API &
          sleep 10  # Wait for API to start

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Run Postman Tests
        run: |
          newman run devshelf.postman_collection.json \
            --environment devshelf-local.postman_environment.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export reports/test-report.html \
            --bail

      - name: Upload Test Report
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: postman-test-report
          path: reports/test-report.html
```

**What `--bail` does:** If any test fails, Newman stops immediately and exits with a non-zero code. GitHub Actions sees this as a failed step. The PR cannot be merged. Your tests are now a hard gate.

---

## Debugging Checklist

When a request fails unexpectedly:

1. **Open Postman Console** (View → Show Postman Console or `Ctrl+Alt+C`)
2. Check what was actually sent vs what you expected
3. Check response headers — often the error is in `Content-Type` or `WWW-Authenticate`
4. Use `console.log()` in your test scripts to debug variable values:
   ```javascript
   console.log("Token value:", pm.environment.get("token"));
   console.log("Full response:", pm.response.json());
   ```
5. If getting 401 unexpectedly — check if the token saved correctly after login
6. If getting 400 — log the request body and confirm JSON is valid

---

## Your Definition of Done for Project 1

Before you call this project complete, every item below must be true:

- [ ] Two environments set up: Local and Staging
- [ ] Zero hardcoded URLs — all use `{{base_url}}`
- [ ] Pre-request script generates unique test data on Register
- [ ] Token saved automatically after Login
- [ ] Minimum 5 test assertions per endpoint
- [ ] Both happy path AND error cases tested (401, 400, 404)
- [ ] Collection runner passes 100% before every commit
- [ ] Data-driven CSV test covers at least 6 edge cases
- [ ] OpenAPI spec imported and schema validation in tests
- [ ] Newman runs successfully from command line
- [ ] GitHub Actions workflow passes on push to main

When all of these are checked, you are genuinely Tier 1 + most of Tier 2 certified.

---

## Key Concepts Reference

### Environment variable scopes

```
Global variables    → available everywhere, any collection
Collection variables → available within one collection
Environment variables → switch with environment selector ← USE THESE
Local variables     → only exist during one request execution
```

### pm object cheatsheet

```javascript
// Response
pm.response.code          // HTTP status number
pm.response.json()        // parsed response body
pm.response.text()        // raw response string
pm.response.responseTime  // milliseconds
pm.response.headers.get("Content-Type")

// Environment
pm.environment.set("key", value)  // save a variable
pm.environment.get("key")         // read a variable

// Tests
pm.test("label", function() { ... })
pm.expect(value).to.equal(expected)
pm.expect(value).to.have.property("field")
pm.expect(value).to.be.greaterThan(0)
pm.expect(value).to.match(/regex/)
pm.expect(array).to.have.lengthOf(5)
pm.expect(value).to.be.a("string")
pm.expect(value).to.include("substring")

// Iteration data (CSV testing)
pm.iterationData.get("columnName")
```

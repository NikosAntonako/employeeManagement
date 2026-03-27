# API Example Requests

## Get All Employees
```
GET /api/employees
```

## Get Employee by ID
```
GET /api/employees/{id}
```

## Create Employee
```
POST /api/employees
Content-Type: application/json

{
  "name": "John Doe",
  "position": "Developer",
  "department": "IT",
  "salary": 60000
}
```

## Update Employee
```
PUT /api/employees/{id}
Content-Type: application/json

{
  "name": "Jane Doe",
  "position": "Manager",
  "department": "HR",
  "salary": 70000
}
```

## Delete Employee
```
DELETE /api/employees/{id}
```

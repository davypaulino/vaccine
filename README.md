https://gist.github.com/DouglasLutz/aa25728e3a438dc966490870f03cc770


## Diagrama de Banco de dados
![Diagrama ER Database](./docs/er-diagram.svg)

<details>
<summary>diagram as code</summary>
<code>

    erDiagram
        PERSON {
            UUID id PK
            string name
            string document
            date birth_date
            datetime created_at
            UUID created_by
            datetime updated_at
            UUID updated_by
        }
    
        USER {
            UUID id PK
            UUID person_id FK
            string email
            string password
            int role
            int status
            datetime created_at
            datetime updated_at
        }
    
        VACCINE {
            UUID id PK
            string name
            int dose_types
            datetime created_at
            UUID created_by
            datetime updated_at
            UUID updated_by
        }
    
        VACCINATION {
            UUID id PK
            UUID person_id FK
            UUID vaccine_id FK
            datetime created_at
            UUID created_by
            datetime updated_at
            UUID updated_by
        }
    
        DOSE {
            UUID id PK
            UUID vaccination_id FK
            int dose_types
            date applied_at
            datetime created_at
            UUID created_by
        }
    
        PERSON ||--|{ VACCINATION : receives
        PERSON o|--o| USER : has_account
        VACCINE ||--|{ VACCINATION : used_in
        VACCINATION ||--o{ DOSE : contains

</code>
</details>
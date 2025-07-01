# 🧪 Pruebas de Integración Nezter - Con Tus Datos

Este documento te guía para ejecutar las pruebas de integración usando los datos específicos de tu base de datos.

## 📋 Datos de Tu Sistema

### 👤 Usuarios Disponibles
- **Admin**: `edgar@gmail.com` / `admin123`
- **Empleado**: `cris@gmail.com` / `empleado123`

### 👥 Empleados Disponibles
- **ID 1**: Carlos Lopez Martinez (Senior Developer)
- **ID 2**: Edgar Lopez Duarte (Senior Developer)

### 🏢 Puestos Disponibles
- **ID 1**: Senior Developer
- **ID 2**: Recursos Humanos

### 🏷️ Clasificaciones Disponibles
- **Determinación** (Rojo)
- **Creatividad** (Naranja)
- **Neutro** (Blanco)
- **Hermetismo** (Verde)
- **Orientación al detalle** (Azul)

### 📝 Exámenes Disponibles
- **ID 1**: Examen de Habilidades Blandas (3 preguntas)
- **ID 2**: Evaluación de Trabajo en Equipo (5 preguntas)

## 🚀 Configuración Rápida

### 1. Importar Colección y Entorno

1. **Abrir Postman**
2. **Importar Colección**:
   - Archivo: `Nezter_API_Collection_Con_Tus_Datos.postman_collection.json`
3. **Importar Entorno**:
   - Archivo: `Nezter_API_Environment_Con_Tus_Datos.postman_environment.json`

### 2. Configurar Entorno

1. En Postman, selecciona el entorno: **"Nezter API - Entorno con Tus Datos"**
2. Verifica que `base_url` sea: `http://localhost:5054`

### 3. Verificar Backend

Asegúrate de que tu backend esté ejecutándose en `http://localhost:5054`

## 🧪 Flujo de Pruebas Recomendado

### Paso 1: Login Admin
- **Endpoint**: `POST /api/auth/login`
- **Datos**: 
  ```json
  {
    "usuario": "edgar@gmail.com",
    "contrasena": "admin123"
  }
  ```
- **Resultado**: Debe devolver un token JWT

### Paso 2: Login Empleado
- **Endpoint**: `POST /api/auth/login`
- **Datos**:
  ```json
  {
    "usuario": "cris@gmail.com",
    "contrasena": "empleado123"
  }
  ```
- **Resultado**: Debe devolver un token JWT

### Paso 3: Asignar Examen
- **Endpoint**: `POST /api/asignaciones/asignar`
- **Datos**:
  ```json
  {
    "idExamen": 1,
    "codigoEmpleado": 1
  }
  ```
- **Resultado**: Debe crear una asignación y devolver su ID

### Paso 4: Responder Examen
- **Endpoint**: `POST /api/resultados/guardar-respuestas-multiples`
- **Datos**:
  ```json
  [
    {
      "idAsignacion": 1,
      "idPregunta": 1,
      "idRespuesta": 5
    },
    {
      "idAsignacion": 1,
      "idPregunta": 2,
      "idRespuesta": 10
    },
    {
      "idAsignacion": 1,
      "idPregunta": 3,
      "idRespuesta": 15
    }
  ]
  ```

### Paso 5: Ver Reporte
- **Endpoint**: `POST /api/resultados/reporte-por-competencia`
- **Datos**:
  ```json
  {
    "idAsignacion": 1
  }
  ```

## 📊 Pruebas Específicas por Módulo

### 🔐 Autenticación
- ✅ Login Admin (edgar@gmail.com)
- ✅ Login Empleado (cris@gmail.com)

### 👥 Empleados
- ✅ Obtener todos los empleados
- ✅ Obtener empleado por código (1)
- ✅ Crear nuevo empleado

### 🏢 Puestos
- ✅ Obtener todos los puestos
- ✅ Crear nuevo puesto

### 📝 Exámenes
- ✅ Obtener todos los exámenes
- ✅ Crear nuevo examen

### 🏷️ Clasificaciones
- ✅ Obtener todas las clasificaciones

### ❓ Preguntas
- ✅ Obtener preguntas por examen (ID: 1)

### 💬 Respuestas
- ✅ Obtener respuestas por pregunta (ID: 1)

### 📋 Asignaciones
- ✅ Asignar examen a empleado
- ✅ Consultar asignaciones por empleado

### 📊 Resultados
- ✅ Guardar respuesta de empleado
- ✅ Guardar múltiples respuestas
- ✅ Obtener reporte por competencia
- ✅ Guardar en historial
- ✅ Obtener historial de empleado

## 🎯 Flujo Completo Automatizado

Usa la carpeta **"🧪 Flujo Completo de Prueba"** que ejecuta automáticamente:
1. Login Admin
2. Asignar Examen
3. Login Empleado
4. Responder Examen
5. Ver Reporte

## 🔍 Verificación de Datos

### Consultas SQL para Verificar

```sql
-- Verificar usuarios
SELECT * FROM Usuarios WHERE usuario IN ('edgar@gmail.com', 'cris@gmail.com');

-- Verificar empleados
SELECT * FROM Empleados;

-- Verificar exámenes
SELECT * FROM Examenes;

-- Verificar preguntas
SELECT * FROM Preguntas WHERE idExamen = 1;

-- Verificar respuestas
SELECT * FROM Respuestas WHERE idPregunta IN (1, 2, 3);

-- Verificar asignaciones
SELECT * FROM Asignaciones;

-- Verificar respuestas de empleado
SELECT * FROM RespuestasEmpleado;

-- Verificar historial
SELECT * FROM HistorialResultados;
```

## ⚠️ Solución de Problemas

### Error 401 Unauthorized
- Verifica que el token JWT esté presente en el header `Authorization`
- Asegúrate de que el usuario exista en la base de datos

### Error 404 Not Found
- Verifica que el `base_url` sea correcto
- Asegúrate de que el backend esté ejecutándose

### Error 500 Internal Server Error
- Revisa los logs del backend
- Verifica que los datos enviados sean correctos

### Variables no se resuelven
- Asegúrate de que el entorno esté seleccionado en Postman
- Verifica que las variables estén definidas correctamente

## 📈 Resultados Esperados

### Login Exitoso
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": 1,
  "username": "edgar@gmail.com",
  "role": "Administrador"
}
```

### Reporte de Competencias
```json
[
  {
    "Competencia": "Creatividad",
    "Promedio": 5.0
  },
  {
    "Competencia": "Orientación al detalle",
    "Promedio": 5.0
  }
]
```

## 🎉 ¡Listo para Probar!

Con estos datos y configuración, puedes ejecutar todas las pruebas de integración en Postman. Los tokens se guardarán automáticamente y se usarán en las siguientes peticiones.

¿Necesitas ayuda con alguna prueba específica? 
import { useState } from "react";

const API_URL = import.meta.env.VITE_API_URL || "";

/**
 * Pregunta 6 — Formulario de registro de una nueva entidad.
 * Framework: React (funcional, con hooks). Sin librerías externas de formularios
 * a propósito, para que el ejercicio muestre el manejo manual de validación,
 * estado de carga y errores que pide la rúbrica.
 */

const CAMPOS_INICIALES = {
  identificacionFiscal: "",
  nombreOficial: "",
  ipPublica: "",
  enlaceTecnico: "",
  correoResponsable: "",
};

const ARCHIVOS_INICIALES = {
  documentoAutorizacion: null,
  resolucionHabilitante: null,
};

const MAX_ARCHIVO_BYTES = 5 * 1024 * 1024; // 5 MB

// --- Validaciones individuales, en lenguaje llano (sin jerga técnica) ---------
function validarIdentificacionFiscal(valor) {
  const identificacion = valor.trim();

  if (!identificacion) {
    return "Ingresa el número de identificación fiscal.";
  }

  if (!/^\d+$/.test(identificacion)) {
    return "El número solo debe contener dígitos.";
  }

  if (identificacion.length < 2) {
    return "El número debe incluir el identificador y su dígito verificador.";
  }

  if (!validarDigitoVerificador(identificacion)) {
    return "El dígito verificador no coincide. Revisa el número ingresado.";
  }

  return "";
}

/**

 * Regla asumida para fines del ejercicio:

 * se utiliza módulo 11 y se considera que el último dígito es el verificador.

 *

 * En una implementación institucional real, el algoritmo debe reemplazarse

 * por la regla oficial definida por la institución o el país correspondiente.

 */
function validarDigitoVerificador(valor) {
  const digitos = valor.slice(0, -1).split("").map(Number);
  const verificador = Number(valor.slice(-1));
  let suma = 0;
  let peso = 2;
  for (let i = digitos.length - 1; i >= 0; i--) {
    suma += digitos[i] * peso;
    peso = peso === 7 ? 2 : peso + 1;
  }
  const resto = (11 - (suma % 11)) % 11;
  const esperado = resto >= 10 ? 0 : resto;
  return esperado === verificador;
}

function validarNombreOficial(valor) {
  if (!valor.trim()) return "Ingresa el nombre oficial de la entidad.";
  if (valor.trim().length < 3) return "El nombre es demasiado corto.";
  return "";
}

function validarIp(valor) {
  const ip = valor.trim();

  if (!ip) {
    return "Ingresa la dirección IP pública del servidor.";
  }

  const partes = ip.split(".");

  const formatoValido =
    partes.length === 4 &&
    partes.every(
      (parte) =>
        /^\d{1,3}$/.test(parte) &&
        Number(parte) >= 0 &&
        Number(parte) <= 255
    );

  if (!formatoValido) {
    return "Ingresa una dirección IPv4 válida, por ejemplo: 200.10.20.30.";
  }

  const [a, b, c, d] = partes.map(Number);

  const esPrivadaOReservada =
    a === 0 ||
    a === 10 ||
    a === 127 ||
    (a === 169 && b === 254) ||
    (a === 172 && b >= 16 && b <= 31) ||
    (a === 192 && b === 168) ||
    (a === 100 && b >= 64 && b <= 127) ||
    a >= 224 ||
    (a === 255 && b === 255 && c === 255 && d === 255);

  if (esPrivadaOReservada) {
    return "La dirección ingresada no corresponde a una IP pública.";
  }

  return "";
}

function validarEnlaceTecnico(valor) {
  if (!valor.trim()) return "Ingresa el nombre del enlace técnico.";
  return "";
}

function validarCorreo(valor) {
  if (!valor.trim()) return "Ingresa el correo del responsable de protección de datos.";
  const formatoValido = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(valor);
  if (!formatoValido) return "El correo no tiene un formato válido.";
  return "";
}

function validarArchivo(archivo, etiqueta) {
  if (!archivo) {
    return `Adjunta el archivo: ${etiqueta}.`;
  }

  const esPdfPorTipo = archivo.type === "application/pdf";
  const esPdfPorNombre = archivo.name.toLowerCase().endsWith(".pdf");

  if (!esPdfPorTipo && !esPdfPorNombre) {
    return `${etiqueta} debe ser un archivo PDF.`;
  }

  if (archivo.size > MAX_ARCHIVO_BYTES) {
    return `${etiqueta} no debe superar 5 MB.`;
  }

  if (archivo.size === 0) {
    return `${etiqueta} está vacío. Selecciona otro archivo.`;
  }

  return "";
}

const VALIDADORES = {
  identificacionFiscal: validarIdentificacionFiscal,
  nombreOficial: validarNombreOficial,
  ipPublica: validarIp,
  enlaceTecnico: validarEnlaceTecnico,
  correoResponsable: validarCorreo,
};

export default function EntidadForm() {
    const [fileInputKey, setFileInputKey] = useState(0);
  const [campos, setCampos] = useState(CAMPOS_INICIALES);
  const [archivos, setArchivos] = useState(ARCHIVOS_INICIALES);
  const [errores, setErrores] = useState({});
  const [enviando, setEnviando] = useState(false);
  const [resultado, setResultado] = useState(null); // { tipo: 'exito' | 'error', mensaje }

  function manejarCambioCampo(evento) {
    const { name, value } = evento.target;
    setCampos((prev) => ({ ...prev, [name]: value }));
    if (errores[name]) {
      setErrores((prev) => ({ ...prev, [name]: "" }));
    }
  }

  function manejarCambioArchivo(evento) {
    const { name, files } = evento.target;
    setArchivos((prev) => ({ ...prev, [name]: files[0] ?? null }));
    if (errores[name]) {
      setErrores((prev) => ({ ...prev, [name]: "" }));
    }
  }

  function validarTodo() {
    const nuevosErrores = {};
    for (const [campo, validador] of Object.entries(VALIDADORES)) {
      const mensaje = validador(campos[campo]);
      if (mensaje) nuevosErrores[campo] = mensaje;
    }
    const errorAutorizacion = validarArchivo(
      archivos.documentoAutorizacion,
      "Documento de autorización institucional"
    );
    if (errorAutorizacion) nuevosErrores.documentoAutorizacion = errorAutorizacion;

    const errorResolucion = validarArchivo(
      archivos.resolucionHabilitante,
      "Resolución o acto administrativo habilitante"
    );
    if (errorResolucion) nuevosErrores.resolucionHabilitante = errorResolucion;

    setErrores(nuevosErrores);
    return Object.keys(nuevosErrores).length === 0;
  }

  async function manejarEnvio(evento) {
    evento.preventDefault();


    if (enviando) return; // Evita envíos múltiples
    
    setResultado(null);


    if (!validarTodo()) return;

    setEnviando(true);
    try {
      const formData = new FormData();
      formData.append("identificacionFiscal", campos.identificacionFiscal);
      formData.append("nombreOficial", campos.nombreOficial);
      formData.append("ipPublica", campos.ipPublica);
      formData.append("enlaceTecnico", campos.enlaceTecnico);
      formData.append("correoResponsable", campos.correoResponsable);
      formData.append("documentoAutorizacion", archivos.documentoAutorizacion);
      formData.append("resolucionHabilitante", archivos.resolucionHabilitante);

      const respuesta = await fetch(`${API_URL}/api/entidades`, {
        method: "POST",
        body: formData,
      });

      if (!respuesta.ok) {
        const cuerpo = await respuesta.json().catch(() => null);

        const mensaje =
          cuerpo?.mensaje ||
          cuerpo?.message ||
          cuerpo?.title ||
          "No se pudo registrar la entidad.";

        throw new Error(mensaje);
      }

      setResultado({ tipo: "exito", mensaje: "La entidad se registró correctamente." });
      setFileInputKey((valorActual) => valorActual + 1);
      setCampos(CAMPOS_INICIALES);
      setArchivos(ARCHIVOS_INICIALES);
    } catch (error) {
      setResultado({
        tipo: "error",
        mensaje: error.message || "Ocurrió un problema al registrar la entidad. Intenta de nuevo.",
      });
    } finally {
      setEnviando(false);
    }
  }

  return (
    <form onSubmit={manejarEnvio} noValidate style={estilos.formulario}>
      <h1 style={estilos.titulo}>Registro de entidad</h1>
      <p style={estilos.subtitulo}>
        Completa los datos y adjunta los documentos requeridos para habilitar el convenio.
      </p>

      <Campo
      
        etiqueta="Número de identificación fiscal"
        name="identificacionFiscal"
        value={campos.identificacionFiscal}
        onChange={manejarCambioCampo}
        error={errores.identificacionFiscal}
        placeholder="Ej: 155123456"
        inputMode="numeric"
        maxLength={20}
        disabled={enviando}
      />

      <Campo
        etiqueta="Nombre oficial de la entidad"
        name="nombreOficial"
        value={campos.nombreOficial}
        onChange={manejarCambioCampo}
        error={errores.nombreOficial}
        placeholder="Ej: Ministerio de Ejemplo"
         maxLength={200}
         disabled={enviando}
      />

      <Campo
        etiqueta="Dirección IP pública del servidor de consumo"
        name="ipPublica"
        value={campos.ipPublica}
        onChange={manejarCambioCampo}
        error={errores.ipPublica}
        placeholder="Ej: 200.10.20.30"
        disabled={enviando}
      />

      <Campo
        etiqueta="Nombre del enlace técnico designado"
        name="enlaceTecnico"
        value={campos.enlaceTecnico}
        onChange={manejarCambioCampo}
        error={errores.enlaceTecnico}
        placeholder="Ej: María González"
        maxLength={100}
        disabled={enviando}
      />

      <Campo
        etiqueta="Correo del responsable de protección de datos"
        name="correoResponsable"
        type="email"
        value={campos.correoResponsable}
        onChange={manejarCambioCampo}
        error={errores.correoResponsable}
        placeholder="nombre@institucion.gob"
        maxLength={100}
        disabled={enviando}
      />

      <CampoArchivo
      inputKey={`autorizacion-${fileInputKey}`}
        etiqueta="Documento de autorización institucional (PDF, máx. 5 MB)"
        name="documentoAutorizacion"
        archivo={archivos.documentoAutorizacion}
        onChange={manejarCambioArchivo}
        error={errores.documentoAutorizacion}
        disabled={enviando} 
      />

      <CampoArchivo
      inputKey={`resolucion-${fileInputKey}`}
        etiqueta="Resolución o acto administrativo habilitante (PDF, máx. 5 MB)"
        name="resolucionHabilitante"
        archivo={archivos.resolucionHabilitante}
        onChange={manejarCambioArchivo}
        error={errores.resolucionHabilitante}
        disabled={enviando}
      />

      {resultado && (
        <div
          role="alert"
          style={resultado.tipo === "exito" ? estilos.mensajeExito : estilos.mensajeError}
        >
          {resultado.mensaje}
        </div>
      )}

      <button type="submit" disabled={enviando} style={estilos.boton}>
        {enviando ? "Registrando..." : "Registrar entidad"}
      </button>
    </form>
  );
}

function Campo({ etiqueta, name, value, onChange, error, placeholder, type = "text",inputMode,

  maxLength,disabled = false, }) {
  return (
    <div style={estilos.grupoCampo}>
      <label htmlFor={name} style={estilos.etiqueta}>
        {etiqueta}
      </label>
      <input
        id={name}
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required
        inputMode={inputMode}
        maxLength={maxLength}
        disabled={disabled}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${name}-error` : undefined}
        style={{ ...estilos.input, ...(error ? estilos.inputConError : {}) }}
      />
      {error && (
        <span id={`${name}-error`} style={estilos.textoError}>
          {error}
        </span>
      )}
    </div>
  );
}

function CampoArchivo({ inputKey, etiqueta, name, archivo, onChange, error ,disabled=false}) {
  return (
    <div style={estilos.grupoCampo}>
      <label htmlFor={name} style={estilos.etiqueta}>
        {etiqueta}
      </label>
      <input
        key={inputKey}
        id={name}
        name={name}
        type="file"
        accept="application/pdf"
        onChange={onChange}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${name}-error` : undefined}
        style={estilos.input}
        disabled={disabled}
      />
      {archivo && !error && <span style={estilos.textoAyuda}>{archivo.name}</span>}
      {error && (
        <span id={`${name}-error`} style={estilos.textoError}>
          {error}
        </span>
      )}
    </div>
  );
}

const estilos = {
  formulario: {
    maxWidth: 480,
    margin: "0 auto",
    padding: 24,
    fontFamily: "system-ui, sans-serif",
  },
  titulo: { fontSize: 22, marginBottom: 4 },
  subtitulo: { fontSize: 14, color: "#555", marginBottom: 20 },
  grupoCampo: { display: "flex", flexDirection: "column", marginBottom: 16 },
  etiqueta: { fontSize: 14, fontWeight: 600, marginBottom: 4 },
  input: {
    padding: "8px 10px",
    fontSize: 14,
    border: "1px solid #ccc",
    borderRadius: 4,
  },
  inputConError: { borderColor: "#c0392b" },
  textoError: { color: "#c0392b", fontSize: 13, marginTop: 4 },
  textoAyuda: { color: "#555", fontSize: 13, marginTop: 4 },
  boton: {
    padding: "10px 16px",
    fontSize: 15,
    fontWeight: 600,
    color: "#fff",
    backgroundColor: "#2d6a4f",
    border: "none",
    borderRadius: 4,
    cursor: "pointer",
  },
  mensajeExito: {
    padding: 10,
    marginBottom: 16,
    backgroundColor: "#e8f5e9",
    color: "#256029",
    borderRadius: 4,
    fontSize: 14,
  },
  mensajeError: {
    padding: 10,
    marginBottom: 16,
    backgroundColor: "#fdecea",
    color: "#c0392b",
    borderRadius: 4,
    fontSize: 14,
  },
};

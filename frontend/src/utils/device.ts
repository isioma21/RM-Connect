/** Turns a User-Agent into e.g. "Edge on Windows". */
export function deviceName(userAgent: string) {
  const browser =
    /Edg\//.test(userAgent) ? 'Edge'
    : /OPR\//.test(userAgent) ? 'Opera'
    : /Firefox\//.test(userAgent) ? 'Firefox'
    : /Chrome\//.test(userAgent) ? 'Chrome'
    : /Safari\//.test(userAgent) ? 'Safari'
    : null
  const system =
    /iPhone|iPad/.test(userAgent) ? 'iPhone'
    : /Android/.test(userAgent) ? 'Android'
    : /Windows/.test(userAgent) ? 'Windows'
    : /Mac OS X/.test(userAgent) ? 'Mac'
    : /Linux/.test(userAgent) ? 'Linux'
    : null

  if (browser && system) return `${browser} on ${system}`
  return browser ?? system ?? 'Unknown device'
}

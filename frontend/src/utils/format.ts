import type { AppointmentChannel } from '../api/types'

// The API sends UTC; the bank works in Lagos time
const timeZone = 'Africa/Lagos'

/** e.g. "Wed 30 Sep" */
export function formatDay(isoDate: string) {
  return new Date(isoDate).toLocaleDateString('en-GB', { timeZone, weekday: 'short', day: 'numeric', month: 'short' })
}

/** e.g. "10:00" */
export function formatTime(isoDate: string) {
  return new Date(isoDate).toLocaleTimeString('en-GB', { timeZone, hour: '2-digit', minute: '2-digit' })
}

export function channelLabel(channel: AppointmentChannel) {
  return channel === 'Call' ? 'Call' : 'Branch visit'
}

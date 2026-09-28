import { Chip } from '@mui/material'
import type { AppointmentStatus, RelationshipStatus } from '../api/types'

const colors: Record<AppointmentStatus | RelationshipStatus, { bg: string; text: string }> = {
  Pending: { bg: '#FBF0DC', text: '#7A4F00' },
  Active: { bg: '#E3EFED', text: '#0F5C55' },
  Booked: { bg: '#E4ECF7', text: '#1F4E8C' },
  Completed: { bg: '#E3F0E5', text: '#2E6B3A' },
  Cancelled: { bg: '#EDEBE6', text: '#5B616B' },
  Declined: { bg: '#EDEBE6', text: '#5B616B' },
  Ended: { bg: '#EDEBE6', text: '#5B616B' },
}

export function StatusChip({ status }: { status: AppointmentStatus | RelationshipStatus }) {
  const { bg, text } = colors[status]
  return <Chip label={status} size="small" sx={{ bgcolor: bg, color: text, fontWeight: 700 }} />
}

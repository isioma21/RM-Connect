import { useState } from 'react'
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack } from '@mui/material'
import { appointmentsApi } from '../../api/appointments'
import type { Appointment } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { SlotPicker } from '../../components/SlotPicker'
import { channelLabel, formatDay, formatTime } from '../../utils/format'

type Props = { appointment: Appointment; onClose: () => void; onRescheduled: () => void }

/** Pick a new date and time for a booked appointment. */
export function RescheduleDialog({ appointment, onClose, onRescheduled }: Props) {
  const [slot, setSlot] = useState<string | null>(null)
  const [error, setError] = useState<unknown>(null)
  const [submitting, setSubmitting] = useState(false)

  async function confirm() {
    if (!slot) return
    setSubmitting(true)
    setError(null)
    try {
      await appointmentsApi.reschedule(appointment.id, slot)
      onRescheduled()
    } catch (err) {
      setError(err)
      setSubmitting(false)
    }
  }

  return (
    <Dialog open onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontFamily: "'Fraunces', Georgia, serif", fontWeight: 600 }}>Reschedule appointment</DialogTitle>
      <DialogContent>
        <Stack spacing={2.5}>
          <DialogContentText>
            Your {channelLabel(appointment.channel).toLowerCase()} is on {formatDay(appointment.startsAt)} at {formatTime(appointment.startsAt)}. Pick a new time.
          </DialogContentText>
          <ErrorAlert error={error} />
          <SlotPicker slot={slot} onSlotChange={setSlot} onError={setError} />
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5 }}>
        <Button onClick={onClose} color="inherit" variant="outlined">Keep current time</Button>
        <Button onClick={confirm} variant="contained" disabled={!slot || submitting}>
          {submitting ? 'Saving…' : 'Confirm new time'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

import { useCallback, useEffect, useState } from 'react'
import { Box, Button, Card, Stack, Typography } from '@mui/material'
import { appointmentsApi } from '../../api/appointments'
import type { Appointment } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { StatusChip } from '../../components/StatusChip'
import { ReasonDialog } from '../../components/ReasonDialog'
import { channelLabel, formatDay, formatTime } from '../../utils/format'

export function ManagerSchedulePage() {
  const [appointments, setAppointments] = useState<Appointment[]>([])
  const [error, setError] = useState<unknown>(null)
  const [now] = useState(() => new Date())
  const [toCancel, setToCancel] = useState<Appointment | null>(null)

  const load = useCallback(() => {
    appointmentsApi.mine().then(setAppointments).catch(setError)
  }, [])

  useEffect(load, [load])

  async function complete(appointment: Appointment) {
    try {
      await appointmentsApi.complete(appointment.id)
      load()
    } catch (err) {
      setError(err)
    }
  }

  async function cancel(reason: string) {
    if (!toCancel) return
    setToCancel(null)
    try {
      await appointmentsApi.cancel(toCancel.id, reason)
      load()
    } catch (err) {
      setError(err)
    }
  }

  const booked = appointments.filter((a) => a.status === 'Booked')
  const readyToComplete = booked.filter((a) => new Date(a.startsAt) <= now)
  const upcoming = booked.filter((a) => new Date(a.startsAt) > now)
  const past = appointments.filter((a) => a.status !== 'Booked').reverse()

  return (
    <Stack spacing={4}>
      <Typography variant="h1">My schedule</Typography>
      <ErrorAlert error={error} />
      {readyToComplete.length > 0 && (
        <Section title="Ready to complete" appointments={readyToComplete} actionLabel="Mark completed" onAction={complete} />
      )}
      <Section title="Upcoming" appointments={upcoming} empty="No upcoming appointments." actionLabel="Cancel" onAction={setToCancel} />
      <Section title="Past" appointments={past} empty="No past appointments yet." />
      {toCancel && (
        <ReasonDialog
          title="Cancel appointment?"
          message={`Are you sure you want to cancel the ${channelLabel(toCancel.channel).toLowerCase()} with ${toCancel.customer.firstName} on ${formatDay(toCancel.startsAt)} at ${formatTime(toCancel.startsAt)}?`}
          label="Reason for the customer"
          helperText={`${toCancel.customer.firstName} will see this.`}
          required
          confirmLabel="Yes, cancel appointment"
          onConfirm={cancel}
          onClose={() => setToCancel(null)}
        />
      )}
    </Stack>
  )
}

type SectionProps = {
  title: string
  appointments: Appointment[]
  empty?: string
  actionLabel?: string
  onAction?: (appointment: Appointment) => void
}

function Section({ title, appointments, empty, actionLabel, onAction }: SectionProps) {
  return (
    <Stack spacing={1.5}>
      <Typography sx={{ fontWeight: 700, color: 'text.secondary' }}>{title}</Typography>
      <Card>
        {appointments.length === 0 && <Typography sx={{ p: 3 }} color="text.secondary">{empty}</Typography>}
        {appointments.map((appointment, index) => (
          <Stack
            key={appointment.id}
            direction={{ xs: 'column', sm: 'row' }}
            spacing={2}
            sx={{ px: { xs: 2, sm: 3 }, py: 2.5, alignItems: { xs: 'flex-start', sm: 'center' }, borderTop: index === 0 ? 0 : 1, borderColor: 'divider' }}
          >
            <Box sx={{ flexGrow: 1 }}>
              <Typography sx={{ fontWeight: 700 }}>
                {formatDay(appointment.startsAt)}, {formatTime(appointment.startsAt)} · {channelLabel(appointment.channel)} · {appointment.customer.firstName} {appointment.customer.lastName}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {appointment.channel === 'Call'
                  ? `Call ${appointment.customer.firstName} on ${appointment.customer.phone}`
                  : `${appointment.manager.branch} branch`}
                {' · '}{appointment.reason}
              </Typography>
              {appointment.cancellationReason && (
                <Typography variant="body2" color="error.main">Cancellation reason: {appointment.cancellationReason}</Typography>
              )}
            </Box>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <StatusChip status={appointment.status} />
              {actionLabel && onAction && (
                <Button variant="outlined" onClick={() => onAction(appointment)}>{actionLabel}</Button>
              )}
            </Stack>
          </Stack>
        ))}
      </Card>
    </Stack>
  )
}
